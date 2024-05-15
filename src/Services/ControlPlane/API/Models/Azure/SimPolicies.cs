namespace ControlPlane.API.Models.Azure
{
    public record SimPoliciesResponse(IEnumerable<SimPolicy> Value);

    public record SimPolicy(
        string Id,
        string Name,
        string Type,
        string Location,
        SystemData SystemData,
        SimPolicyProperties Properties);

    public record SimPolicyProperties(
        UeAmbr UeAmbr,
        ResourceReference DefaultSlice,
        int RegistrationTimer,
        IEnumerable<SliceConfiguration> SliceConfigurations,
        string ProvisioningState,
        Dictionary<string, string> SiteProvisioningState);

    public record UeAmbr(string Uplink, string Downlink);

    public record SliceConfiguration(
        ResourceReference Slice,
        ResourceReference DefaultDataNetwork,
        IEnumerable<DataNetworkConfiguration> DataNetworkConfigurations);

    public record DataNetworkConfiguration(
        ResourceReference DataNetwork,
        UeAmbr SessionAmbr,
        int Q5i,
        int AllocationAndRetentionPriorityLevel,
        string PreemptionCapability,
        string PreemptionVulnerability,
        string DefaultSessionType,
        IEnumerable<string> AdditionalAllowedSessionTypes,
        IEnumerable<ResourceReference> AllowedServices);
}