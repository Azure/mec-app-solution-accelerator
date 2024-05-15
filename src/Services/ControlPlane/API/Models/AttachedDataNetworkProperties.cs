namespace ControlPlane.API.Models
{
    public record AttachedDataNetworkProperties(
        string Name,
        string StaticIpPool
    );
}