using Dapr;
using Dapr.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;
using SolTechnology.Avro;

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

        [Topic("pubsub", "newAlert")]
        [HttpPost]
        public async Task PostAlert(byte[] alertBytes)
        {
            var detection = AvroConvert.Deserialize<DetectedObjectAlert>(alertBytes);
            await _mediator.Send(new PersistAlertCommand()
            {
                Information = detection.Information,
                AlertTriggerTimeFin = DateTime.Now,
                AlertTriggerTimeIni = new DateTime(detection.EveryTime),
                Type = detection.Type,
                Frame = detection.Frame,
            });
            _logger.LogInformation("Stored generic alert");
        }
    }
}