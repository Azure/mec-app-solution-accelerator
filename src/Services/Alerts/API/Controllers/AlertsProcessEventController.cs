using Dapr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;
using MongoDB.Bson.IO;
using SolTechnology.Avro;
using System.Text.RegularExpressions;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.EventControllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlertsProcessEventController : ControllerBase
    {
        private readonly ILogger<AlertsProcessEventController> _logger;
        private readonly IMediator _mediator;
        private readonly IAlertsRepository alertsRepository;

        public AlertsProcessEventController(ILogger<AlertsProcessEventController> logger, IMediator mediator, IAlertsRepository alertsRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.alertsRepository = alertsRepository ?? throw new ArgumentNullException(nameof(alertsRepository));
        }

        [Topic("pubsub", "newAlert")]
        [HttpPost]
        public async Task PostAlert(byte[] alertBytes)
        {
            var paintTime = new StepTime() { StepName = "PaintAlert" , StepStart = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds, };
            var detection = AvroConvert.Deserialize<DetectedObjectAlert>(alertBytes);

            var nFrame = await _mediator.Send(new PaintBoundingBoxesCommand() 
            { 
                BoundingBoxPoints = detection.BoundingBoxes, 
                OriginalImageBase64 = detection.Frame,
            });

            paintTime.StepEnd = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
            detection.TimeTrace.Add(paintTime);
            var saveTime = new StepTimeAsDate() { StepName = "PersistData", StepStart = DateTime.Now };

            var alert = await _mediator.Send(new PersistAlertCommand()
            {
                Information = detection.Information,
                CaptureTime = detection.EveryTime,
                Type = detection.Type,
                Frame = nFrame,
                Accuracy = detection.Accuracy,
                StepTrace = detection.TimeTrace,

            });

            alert.AlertTime = DateTime.Now;

            alert.MsExecutionTime = (alert.AlertTime - alert.CaptureTime).TotalMilliseconds;

            saveTime.StepStop = alert.AlertTime;

            saveTime.StepDuration = (saveTime.StepStop - saveTime.StepStart).TotalMilliseconds;

            var times = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StepTimeAsDate>>(alert.StepTimes);

            times.Add(saveTime);

            alert.StepTimes = Newtonsoft.Json.JsonConvert.SerializeObject(times);

            await this.alertsRepository.Delete(alert.Id);
            await this.alertsRepository.Create(alert);

            _logger.LogInformation("Stored generic alert");
        }
    }
}