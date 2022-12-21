namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Models
{
    public interface IDetectionsRepository : IBaseRepository<Detection, Guid>
    {
        Task CreateInCollection(Detection entity);
        Task<List<Detection>> GetFramesByClassNextInTime(long time, string @class);
    }
}
