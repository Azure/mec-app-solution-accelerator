using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace RtspConverter.Services
{
    public class RtspToHlsEncoderProcess
    {
        private readonly ILogger<RtspToHlsEncoderProcess> logger;
        private readonly RtspToHlsEncoderOptions options;
        private Process? process;
        private bool endProcess = false;

        public RtspToHlsEncoderProcess(RtspToHlsEncoderOptions options,
            ILogger<RtspToHlsEncoderProcess> logger)
        {
            ArgumentNullException.ThrowIfNull(options);
            this.logger = logger;
            this.options = options;
            Start();
        }

        public bool IsRunning()
        {
            return process != null && !process.HasExited;
        }

        public void Kill()
        {
            endProcess = true;
            if (process != null && !process.HasExited)
            {
                logger.LogInformation($"Killing process for camera {options.CameraId}.");
                process.Kill();
            }
        }

        private void Start()
        {
            try
            {
                logger.LogInformation($"Starting process for camera {options.CameraId}");
                CreateDirectoryIfNotExists(options.OutputFolder);
                process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $"-i {options.RtspUri} -c:v libx264 -c:a aac -hls_time 10 -hls_list_size 60 -hls_flags delete_segments -start_number 1 {options.OutputFolder}/stream.m3u8",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    },
                    EnableRaisingEvents = true
                };
                process.Exited += OnProcessExited;
                process.Start();
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Error starting process for camera {options.CameraId}");
            }
        }

        private void OnProcessExited(object? sender, EventArgs e)
        {
            if (!endProcess)
            {
                logger.LogWarning($"Process for camera {options.CameraId} exited unexpectedly.");
                Start();
            }
        }

        private static void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}