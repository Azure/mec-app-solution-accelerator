using Microsoft.EntityFrameworkCore;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Infraestructure
{
    public class AlertsDbContext : DbContext
    {
        public AlertsDbContext(DbContextOptions<AlertsDbContext> options)
            : base(options)
        {

        }
        public DbSet<Alert> Alerts { get; set; }
    }
}