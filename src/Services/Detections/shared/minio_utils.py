import boto3
from botocore.client import Config

class MinIOClient:
    def __init__(self, endpoint_url, access_key, secret_key):
        self.s3 = boto3.client(
            's3',
            endpoint_url=endpoint_url,
            aws_access_key_id=access_key,
            aws_secret_access_key=secret_key,
            config=Config(signature_version='s3v4'),
            region_name='us-east-1'
        )

    def bucket_exists(self, bucket_name):
        try:
            self.s3.head_bucket(Bucket=bucket_name)
            return True
        except:
            return False

    def create_bucket(self, bucket_name):
        self.s3.create_bucket(Bucket=bucket_name)
        print(f"Bucket {bucket_name} created.")

    def object_exists(self, bucket_name, object_name):
        try:
            self.s3.head_object(Bucket=bucket_name, Key=object_name)
            return True
        except:
            return False

    def upload_bytes(self, bucket_name, object_name, data_bytes):
        if not self.bucket_exists(bucket_name):
            self.create_bucket(bucket_name)
        
        if self.object_exists(bucket_name, object_name):
            print(f"Object {object_name} already exists in bucket {bucket_name}. It will be replaced.")

        try:
            self.s3.put_object(Body=data_bytes, Bucket=bucket_name, Key=object_name)
            print(f"Data has been uploaded to {bucket_name} as {object_name}.")
        except Exception as e:
            print(f"An error occurred: {e}")
    
    def download_stream(self, bucket_name, object_name):
        try:
            response = self.s3.get_object(Bucket=bucket_name, Key=object_name)
            streaming_body = response['Body']
            
            all_chunks = b''
            while True:


                chunk = streaming_body.read(1024)  # read only 1KB at a time
                if not chunk:
                    break
                all_chunks += chunk

            return all_chunks  # This will be the entire file in bytes.
            
        except Exception as e:
            print(f"An error occurred: {e}")
            return None
