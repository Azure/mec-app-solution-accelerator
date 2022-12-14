using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Models;

namespace RulesEngine.Infraestructure
{
    public class DetectionsNoSqlRepository : IDetectionsRepository
    {
        public Task Create(Detection entity)
        {
            
            throw new NotImplementedException();
        }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Detection> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Detection>> List()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Detection> List(int skip, int take)
        {
            throw new NotImplementedException();
        }

        public Task Update(Detection entity)
        {
            throw new NotImplementedException();
        }
    }
}
