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
        private List<string> errorMessages = new List<string>();

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
                        Arguments = string.Join(" ", [
                            "-fflags nobuffer",
                            "-loglevel debug",
                            "-rtsp_transport tcp",
                            $"-i {options.RtspUri}",
                            "-vsync 0",
                            "-copyts",
                            "-vcodec copy",
                            "-movflags frag_keyframe+empty_moov",
                            "-an",
                            "-hls_flags delete_segments+append_list",
                            "-f hls",
                            "-hls_time 1",
                            "-hls_list_size 3",
                            "-hls_segment_type mpegts",
                            $"-hls_segment_filename \"{options.OutputFolder}/%d.ts\"",
                            $" \"{options.OutputFolder}/stream.m3u8\""
                        ]),
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                    },
                    EnableRaisingEvents = true
                };
                process.Exited += OnProcessExited;
                process.OutputDataReceived += (sender, args) =>
                {
                    //Ignoring output data to avoid unnecesary logging.
                };
                process.ErrorDataReceived += (sender, args) =>
                {
                    //ffmpeg is printing debug information into error, keeping only last 10 error messages
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        errorMessages.Add(args.Data);
                        if (errorMessages.Count > 10)
                        {
                            errorMessages = errorMessages.TakeLast(10).ToList();
                        }
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
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
                logger.LogError(string.Join("\n", errorMessages));
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