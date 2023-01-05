using Microsoft.AspNetCore.Mvc;
using Dapr;
using MediatR;
using SolTechnology.Avro;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.EventControllers
{
    [ApiController]
    [Route("[controller]")]
    public class DetectionsProcessEventController : ControllerBase
    {
        private readonly ILogger<DetectionsProcessEventController> _logger;
        private readonly IMediator _mediator;

        public DetectionsProcessEventController(ILogger<DetectionsProcessEventController> logger, IMediator mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Topic("pubsub", "newDetection")]
        [HttpPost]
        public async Task DetectionEventHandler(object detectionRaw)
        {
            var detectionStr = detectionRaw.ToString();
            var detectionBytes = AvroConvert.Json2Avro(detectionStr);
            var detection = AvroConvert.Deserialize<ObjectDetected>(detectionBytes);

            var command = new AnalyzeObjectDetectionCommand()
            {
                Id = detection.Id,
                DetectionName = detection.Name,
                EveryTime = detection.EveryTime,
                Frame = detection.Frame,
                DetectionType = detection.Type,
                UrlVideoEncoded = detection.UrlVideoEncoded,
                Classes = detection.Classes,
            };

            await _mediator.Send(command);
        }
    }
}
