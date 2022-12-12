using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;
using MongoDB.Driver;
using SolTechnology.Avro;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<AlertsController> _logger;
        private readonly IAlertsRepository _alertsRepository;

        public AlertsController(DaprClient daprClient, ILogger<AlertsController> logger, IAlertsRepository alertsRepository)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _alertsRepository = alertsRepository ?? throw new ArgumentNullException(nameof(alertsRepository));
        }

        [HttpGet("{skip}/{take}")]
        public async Task<IEnumerable<Alert>> GetPaged(int skip, int take)
        {
            return this._alertsRepository.List(skip, take);
        }

        [HttpGet]
        public async Task<IEnumerable<Alert>> Get()
        {
            return this._alertsRepository.List(0, 10);
        }
    }
}
