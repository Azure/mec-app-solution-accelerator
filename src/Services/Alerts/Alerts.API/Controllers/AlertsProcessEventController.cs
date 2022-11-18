using Dapr;
using Dapr.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.EventControllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlertsProcessEventController : ControllerBase
    {
        private readonly DaprClient _daprClient;
        private readonly ILogger<AlertsProcessEventController> _logger;
        private readonly IMediator _mediator;

        public AlertsProcessEventController(DaprClient daprClient, ILogger<AlertsProcessEventController> logger, IMediator mediator)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Topic("pubsub", "newDetection")]
        [HttpPost]
        public async Task PostDetection(object alertRaw)
        {
            await _mediator.Send(new PersistAlertCommand()
            {
                Information = $"Alert {alertRaw}",
                Accuracy = 90,
                AlertTriggerTimeFin = DateTime.Now,
                AlertTriggerTimeIni = DateTime.Now,
                UrlVideoEncoded = "Example",
                lat = 100,
                @long = 100,
            });
        }

        [Topic("pubsub", "newAlertDotNet")]
        [HttpPost("new")]
        public async Task<Guid> Post(Alert alert)
        {
            alert.AlertTriggerTimeIni = DateTime.UtcNow;
            var id = Guid.NewGuid();
            alert.Id = id;
            await _daprClient.SaveStateAsync("statestore", "lastalert", alert);

            return id;
        }
    }
}
