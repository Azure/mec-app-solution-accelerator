using System.Collections.Concurrent;
using System.Diagnostics;
using RtspConverter.Services;

namespace RtspConverter.Models
{
    public class SharedCameraState
    {
        public ConcurrentDictionary<string, CameraInfo> Cameras { get; } = new ConcurrentDictionary<string, CameraInfo>();
    }

    public class CameraInfo
    {
        public string Id { get; set; }
        public string RtspUri { get; set; }
        public bool ToDelete { get; set; }
        public RtspToHlsEncoderProcess Process { get; set; }

    }
}