using MediatR;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Commands
{
    public class PaintBoundingBoxesCommand : IRequest<string>
    {
        public PaintBoundingBoxesCommand()
        {
        }

        public string OriginalImageBase64 { get; set; }
        public List<DetectionClass> MatchingClasses { get; set; }
    }
}
