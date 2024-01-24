namespace ControlPlane.API.Models.Azure
{
    public record SimsResponse(IEnumerable<Sim> Value);

    public record CreateSimRequest(
        CreateSimProperties Properties
    );

    public record CreateSimProperties(
        string IntegratedCircuitCardIdentifier,
        string InternationalMobileSubscriberIdentity,
        string AuthenticationKey,
        string OperatorKeyCode,
        string DeviceType,
        ResourceReference? SimPolicy,
        IEnumerable<StaticIpConfiguration>? StaticIpConfiguration
    );

    public record Sim(
        string Id,
        string Name,
        string Type,
        SystemData SystemData,
        SimProperties Properties);

    public record SimProperties(
        string ProvisioningState,
        string SimState,
        Dictionary<string, string> SiteProvisioningState,
        string InternationalMobileSubscriberIdentity,
        string? IntegratedCircuitCardIdentifier,
        string? AuthenticationKey,
        string? OperatorKeyCode,
        string DeviceType,
        ResourceReference SimPolicy,
        IEnumerable<StaticIpConfiguration> StaticIpConfiguration);

    public record StaticIpConfiguration(
        ResourceReference AttachedDataNetwork,
        ResourceReference Slice,
        StaticIp StaticIp);


    public record StaticIp(string Ipv4Address);
}