using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using RtspConverter.Models;

namespace RtspConverter.Services
{
    public class CameraTrackingService : BackgroundService
    {
        private static readonly int TASK_DELAY_IN_MS = 1000;
        private readonly ILogger<CameraTrackingService> logger;
        private readonly SharedCameraState sharedCameraState;
        private IMongoCollection<BsonDocument> cameraCollection;

        public CameraTrackingService(ILogger<CameraTrackingService> logger,
            SharedCameraState sharedCameraState,
            IMongoCollection<BsonDocument> cameraCollection)
        {
            this.logger = logger;
            this.sharedCameraState = sharedCameraState;
            this.cameraCollection = cameraCollection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    logger.LogInformation("Querying database for cameras update");
                    var camerasInDb = await cameraCollection.Find(_ => true).ToListAsync(stoppingToken);

                    MarkToDeleteRemovedCameras(camerasInDb.Select(c => c["_id"].AsString).ToHashSet());
                    ExposeCameraInfo(camerasInDb);
                }
                catch (Exception ex)
                {
                    logger.LogError($"An error occurred while processing cameras: {ex.Message}");
                }

                await Task.Delay(TASK_DELAY_IN_MS, stoppingToken);
            }
        }

        private void MarkToDeleteRemovedCameras(HashSet<string> expectedCameras)
        {
            foreach (var cameraId in sharedCameraState.Cameras.Keys.ToList())
            {
                if (!expectedCameras.Contains(cameraId))
                {
                    if (sharedCameraState.Cameras.TryGetValue(cameraId, out var cameraInfo))
                    {
                        logger.LogInformation($"Camera {cameraId} not found in database. Marking for deletion or stopping processing.");
                        cameraInfo.ToDelete = true;
                    }
                }
            }
        }

        private void ExposeCameraInfo(List<BsonDocument> cameraInfoList)
        {
            foreach (var camera in cameraInfoList)
            {
                var cameraId = camera["_id"].AsString;
                var rtspUrl = camera["Rtsp"].AsString;

                if (!sharedCameraState.Cameras.ContainsKey(cameraId))
                {
                    sharedCameraState.Cameras.TryAdd(cameraId, new CameraInfo()
                    {
                        Id = cameraId,
                        RtspUri = rtspUrl,
                    });
                    logger.LogInformation($"Added camera {cameraId} for processing.");
                }
            }
        }
    }
}