using Dapr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events;
using SolTechnology.Avro;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.EventControllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlertsProcessEventController : ControllerBase
    {
        private readonly ILogger<AlertsProcessEventController> _logger;
        private readonly IMediator _mediator;

        public AlertsProcessEventController(ILogger<AlertsProcessEventController> logger, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Topic("pubsub", "newAlert")]
        [HttpPost]
        public async Task PostAlert(byte[] alertBytes)
        {
            var now = DateTime.Now;
            var detection = AvroConvert.Deserialize<DetectedObjectAlert>(alertBytes);
            detection.TimeTrace.FirstOrDefault(t => t.StepName.Equals("RulesEngine")).StepEnd = (long)(now - new DateTime(1970, 1, 1)).TotalMilliseconds;
            var alert = await _mediator.Send(new PersistAlertCommand()
            {
                Information = detection.Information,
                CaptureTime = detection.EveryTime,
                Type = detection.Type,
                Frame = detection.Frame,
                Accuracy = detection.Accuracy,
                StepTrace = detection.TimeTrace,
                MatchingClasses = detection.MatchingClasses,
            });

            _logger.LogInformation("Stored generic alert");
        }
    }
}