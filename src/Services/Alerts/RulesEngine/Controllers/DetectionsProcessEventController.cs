using Microsoft.AspNetCore.Mvc;
using Dapr;
using MediatR;
using SolTechnology.Avro;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;
using Alerts.RulesEngine.Commands;

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
        [HttpPost("newDetection")]
        public async Task<IActionResult> DetectionEventHandler(object detectionRaw)
        {
            try
            {
                var stepTime = new StepTime { StepName = "RulesEngine", StepStart = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds };
                var detectionStr = detectionRaw.ToString();
                var detectionBytes = AvroConvert.Json2Avro(detectionStr);
                var detection = AvroConvert.Deserialize<ObjectDetected>(detectionBytes);
                _logger.LogInformation($"Received detection at {detection.EveryTime} from {detection.SourceId}");
                detection.time_trace.Add(stepTime);
                var command = new AnalyzeObjectDetectionCommand()
                {
                    Id = detection.Id,
                    DetectionName = detection.Name,
                    EveryTime = detection.EveryTime,
                    Frame = detection.Frame,
                    DetectionType = detection.Type,
                    UrlVideoEncoded = detection.UrlVideoEncoded,
                    Classes = detection.Classes,
                    TimeTrace = detection.time_trace,
                    SourceId = detection.SourceId
                };

                await _mediator.Send(command);

                return Ok();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while processing the detection event");
                return BadRequest("Invalid detection event: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the detection event");
                return StatusCode(500, "An error occurred while processing the detection event");

            }
        }
    }
}
