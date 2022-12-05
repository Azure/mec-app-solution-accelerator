using Dapr;
using Dapr.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;
using SolTechnology.Avro;
using System;
using System.Globalization;

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
        public async Task PostDetection(object detectionRaw)
        {
            var detectionStr = detectionRaw.ToString();
            var detectionBytes = AvroConvert.Json2Avro(detectionStr);
            var detection = AvroConvert.Deserialize<ObjectDetected>(detectionBytes);
            await _mediator.Send(new PersistAlertCommand()
            {
                Information = detection.Information,
                Frame = detection.Frame,
                AlertTriggerTimeFin = DateTime.Now,
                AlertTriggerTimeIni = new DateTime(detection.EveryTime),
                UrlVideoEncoded = detection.UrlVideoEncoded,
                Type = detection.EventType,
            }); ;

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
