namespace ControlPlane.API.Models.Azure
{
    public record SystemData(
        string CreatedBy,
        string CreatedByType,
        DateTime CreatedAt,
        string LastModifiedBy,
        string LastModifiedByType,
        DateTime LastModifiedAt);

    public record ResourceReference(string Id);
}