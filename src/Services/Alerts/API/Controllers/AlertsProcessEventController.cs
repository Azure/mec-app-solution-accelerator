using Dapr;
using Dapr.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events.Base;
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
            var detection = AvroConvert.Deserialize<DetectedObjectAlert>(alertBytes);
            var bytes = Convert.FromBase64String(detection.Frame);
            using( var origStream = new StreamContent(new MemoryStream(bytes)))
            {
                this.PaintBoundingBoxes(origStream, detection.BoundingBoxes);
            }
            var contents = new StreamContent(new MemoryStream(bytes));

            await _mediator.Send(new PersistAlertCommand()
            {
                Information = detection.Information,
                CaptureTime = detection.EveryTime,
                Type = detection.Type,
                Frame = detection.Frame,
                Accuracy = detection.Accuracy
            });
            _logger.LogInformation("Stored generic alert");
        }

        private string PaintBoundingBoxes(MemoryStream image, List<BoundingBox> boxes)
        {
            foreach(BoundingBox box in boxes)
            {

            }

            return "";
        }

    }
}