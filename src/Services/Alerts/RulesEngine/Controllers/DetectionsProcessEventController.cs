using Microsoft.AspNetCore.Mvc;
using Dapr;
using MediatR;
using SolTechnology.Avro;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.EventControllers
{
    [ApiController]
    [Route("[controller]")]
    public class DetectionsProcessEventController : ControllerBase
    {
        private readonly ILogger<DetectionsProcessEventController> _logger;
        private readonly IMediator _mediator;
        private readonly Dictionary<string, System.Type> _commandsTypeByDetectionName;

        public DetectionsProcessEventController(ILogger<DetectionsProcessEventController> logger, IMediator mediator, Dictionary<string, System.Type> commandsTypeByDetectionName)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _commandsTypeByDetectionName = commandsTypeByDetectionName ?? throw new ArgumentNullException(nameof(commandsTypeByDetectionName));
        }

        [Topic("pubsub", "newDetection")]
        [HttpPost]
        public async Task PostDetection(object detectionRaw)
        {
            var detectionStr = detectionRaw.ToString();
            var detectionBytes = AvroConvert.Json2Avro(detectionStr);
            var detection = AvroConvert.Deserialize<ObjectDetected>(detectionBytes);

            var defaultEvent = _commandsTypeByDetectionName.ContainsKey(detection.EventType) ? 
                _commandsTypeByDetectionName[detection.EventType] : _commandsTypeByDetectionName["default"];

            dynamic command = Activator.CreateInstance(defaultEvent);
            command.Information = detection.Information;
            command.Frame = detection.Frame;
            command.Id = detection.Id;
            command.UrlVideoEncoded = detection.UrlVideoEncoded;
            command.Frame = detection.Frame;
            command.EventType = detection.EventType;
            command.EventName = detection.EventName;
            command.SourceId = detection.SourceId;
            command.EveryTime = detection.EveryTime;
            command.Type = detection.Type;
            command.BoundingBoxes = detection.BoundingBoxes;
            await _mediator.Send(command);
        }
    }
}
