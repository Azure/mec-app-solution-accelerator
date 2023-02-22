using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using System.Numerics;
using MediatR;
using Alerts.API.Configuration;


namespace Microsoft.MecSolutionAccelerator.Services.Alerts.CommandHandlers
{
    public class PaintBoundingBoxesCommandHandler : IRequestHandler<PaintBoundingBoxesCommand, string>
    {
        private readonly ColorBoundingBoxConfiguration _colorsConfiguration;
        private const int maxwidth = 1920;
        private const int maxheight = 1080;
        public PaintBoundingBoxesCommandHandler(ColorBoundingBoxConfiguration colorsConfiguration)
        {
            _colorsConfiguration = colorsConfiguration ?? throw new ArgumentNullException(nameof(colorsConfiguration));
        }

        public async Task<string> Handle(PaintBoundingBoxesCommand request, CancellationToken cancellationToken)
        {
            var bytes = Convert.FromBase64String(request.OriginalImageBase64);
            var nFrame = string.Empty;
            using (var origStream = new MemoryStream(bytes))
            {
                nFrame = this.PaintBoundingBoxes(origStream, request.MatchingClasses);
            }
            return nFrame;
        }

        private string PaintBoundingBoxes(MemoryStream stream, List<DetectionClass> matchingClasses)
        {
            var c = new List<List<Vector2>>();
            var firstLane = new PointF[2];
            var secondLane = new PointF[2];
            var thirdLane = new PointF[2];
            var fourthLane = new PointF[2];
            var imgResult = string.Empty;
            var colorDefault = Brushes.Solid(Color.Yellow);

            using (var image = Image.Load(stream))
            {
                foreach(var matchingClass in matchingClasses)
                {
                    firstLane[0] = new Vector2(matchingClass.BoundingBoxes[0].x, matchingClass.BoundingBoxes[0].y);
                    firstLane[1] = new Vector2(matchingClass.BoundingBoxes[1].x, matchingClass.BoundingBoxes[1].y);

                    secondLane[0] = firstLane[1];
                    secondLane[1] = new Vector2(matchingClass.BoundingBoxes[3].x, matchingClass.BoundingBoxes[3].y);

                    thirdLane[0] = secondLane[1];
                    thirdLane[1] = new Vector2(matchingClass.BoundingBoxes[2].x, matchingClass.BoundingBoxes[2].y);

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
                }

                if (image.Width > maxwidth || image.Height > maxheight)
                {
                    int width = Math.Min(image.Width, maxwidth);
                    int height = Math.Min(image.Height, maxheight);
                    image.Mutate(x => x.Resize(new Size(width, height)));
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
