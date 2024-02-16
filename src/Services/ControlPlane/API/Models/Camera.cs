namespace ControlPlane.API.Models
{
    public record Camera(
        string Id,
        string? Model,
        string? Type,
        string? Ip,
        string? Port,
        string? SimId,
        string? Rtsp,
        string? Hls,
        string? Username,
        string? Password
    );
}