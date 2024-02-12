namespace ControlPlane.API.Models.Azure
{
    public record AttachedDataNetworkResponse(
        string Id,
        string Name,
        string Type,
        string Location,
        SystemData SystemData,
        AttachedDataNetworkProperties Properties
    );

    public record AttachedDataNetworkProperties(
        string ProvisioningState,
        UserPlaneDataInterface UserPlaneDataInterface,
        List<string> DnsAddresses,
        NaptConfiguration NaptConfiguration,
        List<string> UserEquipmentAddressPoolPrefix,
        List<string> UserEquipmentStaticAddressPoolPrefix
    );

    public record UserPlaneDataInterface(
        string Name
    );

    public record NaptConfiguration(
        string Enabled
    );
}