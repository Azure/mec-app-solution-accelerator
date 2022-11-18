using Dapr;
using Dapr.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Queries;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlertsController : ControllerBase
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<AlertsController> _logger;
        private readonly IMediator _mediator;

        public AlertsController(DaprClient daprClient, ILogger<AlertsController> logger, IMediator mediator)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IEnumerable<Alert>> Get()
        {
            return await this._mediator.Send(new GetAlertsQuery());
        }

        [HttpGet("last")]
        public async Task<Alert> GetAlert()
            => await _daprClient.GetStateAsync<Alert>("statestore", "lastalert");

        [HttpPost("queue")]
        public async Task QueueAlert(Alert alert)
            => await _daprClient.PublishEventAsync("pubsub", "newAlertDotNet", alert);

    }
}
