using MediatR;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Commands
{
    public class PaintBoundingBoxesCommand : IRequest<string>
    {
        public PaintBoundingBoxesCommand()
        {
        }

        public string OriginalImageBase64 { get; set; }
        public IEnumerable<DetectionClass> MatchingClasses { get; set; }
    }
}
