using Microsoft.EntityFrameworkCore;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Infraestructure
{
    public class AlertsRepository : IAlertsRepository
    {
        private readonly AlertsDbContext _context;
        public AlertsRepository(AlertsDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task Create(Alert entity)
        {
            await _context.Alerts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Guid id)
        {
            var entity = await this.GetById(id);
            await _context.SaveChangesAsync();
        }
        public async Task<Alert> GetById(Guid id)
        {
            return await _context.Alerts.FindAsync(id);
        }
        public async Task<IEnumerable<Alert>> List()
        {
            return await _context.Alerts.ToListAsync();
        }

        public async Task<IEnumerable<Alert>> List(int skip, int take)
        {
            return await _context.Alerts.Skip(skip).Take(take).ToListAsync();
        }

        public async Task Update(Alert entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
