using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events.Base;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System.Numerics;
using MediatR;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.CommandHandlers
{
    public class PaintBoundingBoxesCommandHandler : IRequestHandler<PaintBoundingBoxesCommand, string>
    {
        public async Task<string> Handle(PaintBoundingBoxesCommand request, CancellationToken cancellationToken)
        {
            var bytes = Convert.FromBase64String(request.OriginalImageBase64);
            var nFrame = string.Empty;
            using (var origStream = new MemoryStream(bytes))
            {
                nFrame = this.PaintBoundingBoxes(origStream, request.BoundingBoxPoints);
            }
            return nFrame;
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
            var colorDefault = Brushes.Solid(Color.Yellow);

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

                    image.Mutate(x => x.DrawLines(colorDefault, 5, firstLane.ToArray()));
                    image.Mutate(x => x.DrawLines(colorDefault, 5, secondLane.ToArray()));
                    image.Mutate(x => x.DrawLines(colorDefault, 5, thirdLane.ToArray()));
                    image.Mutate(x => x.DrawLines(colorDefault, 5, fourthLane.ToArray()));
                    firstLane = new PointF[2];
                    secondLane = new PointF[2];
                    thirdLane = new PointF[2];
                    fourthLane = new PointF[2];

                    skip += 4;

                }

                int width = image.Width / 4;
                int height = image.Height / 4;
                image.Mutate(x => x.Resize(width, height));

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
