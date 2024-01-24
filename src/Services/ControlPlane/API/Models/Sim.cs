namespace ControlPlane.API.Models
{
    public record Sim(
    string Name,
    string Iccid,
    string Imsi,
    string Opc,
    string GroupId,
    string Ki,
    string? Ip,
    string? PolicyId);
}