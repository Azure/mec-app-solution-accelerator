namespace ControlPlane.API.Models.Azure
{
    public record SimGroupsResponse(IEnumerable<SimGroup> Value);

    public record SimGroup(
        string Id,
        string Name,
        string Type,
        string Location,
        SystemData SystemData,
        SimGroupProperties Properties);

    public record SimGroupProperties(
        string ProvisioningState,
        MobileNetworkReference MobileNetwork);

    public record MobileNetworkReference(string Id);
}