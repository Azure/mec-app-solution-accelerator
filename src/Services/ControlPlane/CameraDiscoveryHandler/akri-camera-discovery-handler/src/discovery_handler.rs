use akri_discovery_utils::discovery::{
    discovery_handler::DISCOVERED_DEVICES_CHANNEL_CAPACITY,
    v0::{discovery_handler_server::DiscoveryHandler, Device, DiscoverRequest, DiscoverResponse},
    DiscoverStream,
};
use async_trait::async_trait;
use bson::Document;
use mongodb::{options::ClientOptions, Client, Collection};
use std::collections::HashMap;
use std::{thread, time};
use tokio::sync::mpsc;
use tokio_stream::StreamExt;
use tonic::{Response, Status};

const BROKER_NAME: &str = "AKRI_MEC";
const RTSP_URI_NAME: &str = "RTSP_URI";
const MODEL_NAME: &str = "CAMERA_MODEL";
const CAMERA_ID: &str = "CAMERA_ID";

/// This defines the MongoDB data stored in the Configuration
#[derive(Serialize, Deserialize, Clone, Debug)]
#[serde(rename_all = "camelCase")]
pub struct MecDiscoveryDetails {
    #[serde(default)]
    pub connection_string: String,
    #[serde(default)]
    pub database: String,
    #[serde(default)]
    pub collection: String,
}

pub fn parse_mec_discovery_details(input: &str) -> Result<MecDiscoveryDetails, String> {
    if !input.starts_with("map[") || !input.ends_with("]") {
        return Err("Invalid format".to_string());
    }

    let inner_content = &input[4..input.len() - 1];
    let pairs = inner_content.split_whitespace();
    let mut map = HashMap::new();

    for pair in pairs {
        let mut kv = pair.splitn(2, ':');
        match (kv.next(), kv.next()) {
            (Some(key), Some(value)) => {
                map.insert(key.to_string(), value.to_string());
            }
            _ => return Err("Invalid key-value pair".to_string()),
        }
    }

    let collection = map
        .get("collection")
        .ok_or_else(|| "Missing 'collection' key".to_string())?
        .to_string();

    let connection_string = map
        .get("connectionString")
        .ok_or_else(|| "Missing 'connectionString' key".to_string())?
        .to_string();

    let database = map
        .get("database")
        .ok_or_else(|| "Missing 'database' key".to_string())?
        .to_string();

    Ok(MecDiscoveryDetails {
        collection,
        connection_string,
        database,
    })
}

pub struct DiscoveryHandlerImpl {
    register_sender: tokio::sync::mpsc::Sender<()>,
}

impl DiscoveryHandlerImpl {
    pub fn new(register_sender: tokio::sync::mpsc::Sender<()>) -> Self {
        DiscoveryHandlerImpl { register_sender }
    }
}

#[async_trait]
impl DiscoveryHandler for DiscoveryHandlerImpl {
    type DiscoverStream = DiscoverStream;
    async fn discover(
        &self,
        request: tonic::Request<DiscoverRequest>,
    ) -> Result<Response<Self::DiscoverStream>, Status> {
        let discover_request = request.get_ref();
        println!("Configuration:");
        println!("{}", discover_request.discovery_details.clone());

        let discovery_handler_config: MecDiscoveryDetails =
            parse_mec_discovery_details(&discover_request.discovery_details).map_err(|e| {
                print!("{}", e);
                tonic::Status::new(tonic::Code::InvalidArgument, format!("{}", e))
            })?;

        let client_options =
            ClientOptions::parse(discovery_handler_config.connection_string.clone())
                .await
                .unwrap();
        let client = Client::with_options(client_options).unwrap();
        let db = client.database(discovery_handler_config.database.as_str());
        let collection: Collection<Document> =
            db.collection(discovery_handler_config.collection.as_str());

        // Create a channel for sending and receiving device updates
        let (stream_sender, stream_receiver) = mpsc::channel(DISCOVERED_DEVICES_CHANNEL_CAPACITY);
        let register_sender = self.register_sender.clone();
        tokio::spawn(async move {
            loop {
                println!("Querying devices:");
                // Response is a newline separated list of devices (host:port) or empty
                let mut cursor = collection.find(None, None).await.unwrap();
                let mut devices = Vec::new();

                while let Some(result) = cursor.next().await {
                    if let Ok(document) = result {
                        let device_id = document
                            .get("_id")
                            .and_then(bson::Bson::as_str)
                            .unwrap_or_default()
                            .to_string();
                        let endpoint = document.get_str("Rtsp").unwrap_or_default().to_string();
                        let model = document.get_str("Model").unwrap_or_default().to_string();

                        let mut properties = HashMap::new();
                        properties.insert(BROKER_NAME.to_string(), "mec".to_string());
                        properties.insert(RTSP_URI_NAME.to_string(), endpoint.clone());
                        properties.insert(MODEL_NAME.to_string(), model.clone());
                        properties.insert(CAMERA_ID.to_string(), device_id.clone());
                        println!("{:?}", properties);

                        let device = Device {
                            id: device_id,
                            properties,
                            mounts: Vec::default(),
                            device_specs: Vec::default(),
                        };

                        devices.push(device);
                    }
                }
                // Send the Agent the list of devices.
                if let Err(_) = stream_sender
                    .send(Ok(DiscoverResponse {
                        devices: devices.to_vec(),
                    }))
                    .await
                {
                    // Agent dropped its end of the stream. Stop discovering and signal to try to re-register.
                    register_sender.send(()).await.unwrap();
                    break;
                }

                thread::sleep(time::Duration::from_secs(5));
            }
        });

        // Send the agent one end of the channel to receive device updates
        Ok(Response::new(tokio_stream::wrappers::ReceiverStream::new(
            stream_receiver,
        )))
    }
}
