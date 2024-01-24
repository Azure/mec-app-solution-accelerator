namespace ControlPlane.API.Models
{
    public record Camera(
        string Id,
        string Model,
        CameraType Type,
        string Ip,
        string Port,
        string? simId,
        string? Rtsp,
        string? Hls
    );
}