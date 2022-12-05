using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;
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

        [HttpGet("{Skip}/{Take}")]
        public async Task<IEnumerable<Alert>> GetPaged(int skip, int take)
        {
            return await this._alertsRepository.List(skip, take);
        }

        [HttpGet]
        public async Task<IEnumerable<Alert>> Get()
        {
            return await this._alertsRepository.List(0, 10);
        }

        [HttpGet("last")]
        public async Task<Alert> GetAlert()
            => await _daprClient.GetStateAsync<Alert>("statestore", "lastalert");

        [HttpPost("queue")]
        public async Task QueueAlert(ObjectDetected detection)
        {
            var serialized = AvroConvert.Serialize(detection);
            await _daprClient.PublishEventAsync("pubsub", "objectDetected", Convert.ToBase64String(serialized));
        }

    }
}
