using Dapr;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events.Base;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SolTechnology.Avro;
using System.Numerics;

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
            var nFrame = string.Empty;
            using (var origStream = new MemoryStream(bytes))
            {
               nFrame = this.PaintBoundingBoxes(origStream, detection.BoundingBoxes);
            }

            await _mediator.Send(new PersistAlertCommand()
            {
                Information = detection.Information,
                CaptureTime = detection.EveryTime,
                Type = detection.Type,
                Frame = nFrame,
                Accuracy = detection.Accuracy
            });
            _logger.LogInformation("Stored generic alert");
        }

        private string PaintBoundingBoxes(MemoryStream stream, List<BoundingBoxPoint> boxes)
        {
            var skip = 0;
            var take = 4;
            var c = new List<List<Vector2>>();
            var firstLane = new PointF[2];
            var secondLane = new PointF[2];
            var thirdLane = new PointF[2];
            var fourthLane = new PointF[2];
            var imgResult = string.Empty;
            var color = Brushes.Solid(Color.Black);

            using (var image = Image.Load(stream))
            {
                while (skip < boxes.Count)
                {
                    var points = boxes.Skip(skip).Take(take).ToArray();
                    firstLane[0] = new Vector2(points[0].x, points[0].y);
                    firstLane[1] = new Vector2(points[1].x, points[1].y);

                    secondLane[0] = firstLane[1];
                    secondLane[1] = new Vector2(points[3].x, points[3].y);

                    thirdLane[0] = secondLane[1];
                    thirdLane[1] = new Vector2(points[2].x, points[2].y);

                    fourthLane[0] = thirdLane[1];
                    fourthLane[1] = firstLane[0];

                    image.Mutate(x => x.DrawLines(color, 5, firstLane.ToArray()));
                    image.Mutate(x => x.DrawLines(color, 5, secondLane.ToArray()));
                    image.Mutate(x => x.DrawLines(color, 5, thirdLane.ToArray()));
                    image.Mutate(x => x.DrawLines(color, 5, fourthLane.ToArray()));
                    firstLane = new PointF[2];
                    secondLane = new PointF[2];
                    thirdLane = new PointF[2];
                    fourthLane = new PointF[2];

                    skip += 4;

                }

                using (var streamResult = new MemoryStream())
                {
                    image.SaveAsJpeg(streamResult);
                    var bytes = streamResult.ToArray();
                    imgResult = Convert.ToBase64String(bytes);
                }
            }

            return imgResult;
        }

    }
}