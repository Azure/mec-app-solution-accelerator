using Coravel.Invocable;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.API.Jobs
{
    public class CosmosDbCleanupJob : IInvocable
    {
        private readonly ILogger<CosmosDbCleanupJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        public CosmosDbCleanupJob(ILogger<CosmosDbCleanupJob> logger, IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task Invoke()
        {
            _logger.LogInformation("MongoDB Cleaning executing...");

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var alertsRepository = scope.ServiceProvider.GetRequiredService<IAlertsRepository>();
                    await alertsRepository.DropData();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while executing the MongoDB Cleaning.");
            }

            _logger.LogInformation("MongoDB Cleaning executed.");
        }
    }
}
