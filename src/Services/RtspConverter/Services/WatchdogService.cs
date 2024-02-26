using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RtspConverter.Models;

namespace RtspConverter.Services
{
    public class WatchdogService : BackgroundService
    {
        private static readonly int TASK_DELAY_IN_MS = 500;
        private static readonly string VIDEO_OUTPUT_PATH = "/var/www/hls";
        private readonly ILogger<WatchdogService> logger;
        private readonly SharedCameraState sharedState;
        private IServiceProvider serviceProvider;


        public WatchdogService(ILogger<WatchdogService> logger,
        IServiceProvider serviceProvider,
        SharedCameraState sharedState)
        {
            this.logger = logger;
            this.sharedState = sharedState;
            this.serviceProvider = serviceProvider;
        }

        public async Task ProcessCamera(CameraInfo cameraInfo)
        {
            try
            {
                // If marked for deletion, stop the process.
                if (cameraInfo.ToDelete)
                {
                    cameraInfo.Process?.Kill();
                    return;
                }

                if (cameraInfo.Process == null && string.IsNullOrEmpty(cameraInfo.HlsUri))
                {
                    cameraInfo.Process = new RtspToHlsEncoderProcess(new RtspToHlsEncoderOptions()
                    {
                        CameraId = cameraInfo.Id,
                        OutputFolder = $"{VIDEO_OUTPUT_PATH}/{cameraInfo.Id}",
                        RtspUri = cameraInfo.RtspUri
                    }, serviceProvider.GetService<ILogger<RtspToHlsEncoderProcess>>());
                }
            }
            catch (Exception e)
            {
                logger.LogError($"Error processing camera ${cameraInfo.Id}", e);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var camera in sharedState.Cameras)
                {
                    var cameraInfo = camera.Value;
                    await ProcessCamera(cameraInfo);
                }

                // Remove cameras marked for deletion
                sharedState.Cameras
                    .Where(x => x.Value.ToDelete)
                    .Select(x => x.Value.Id)
                    .ToList()
                    .ForEach(cameraId =>
                    {
                        if (!string.IsNullOrEmpty(cameraId))
                        {
                            sharedState.Cameras.TryRemove(cameraId, out var _);
                            logger.LogInformation($"Camera {cameraId} removed from monitoring.");
                        }
                    });

                await Task.Delay(TASK_DELAY_IN_MS, stoppingToken);
            }
        }
    }
}