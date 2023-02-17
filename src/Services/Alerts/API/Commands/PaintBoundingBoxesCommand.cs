using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events.Base;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Commands
{
    public class PaintBoundingBoxesCommand : IRequest<string>
    {
        public PaintBoundingBoxesCommand()
        {
        }

        public string OriginalImageBase64 { get; set; }
        public List<BoundingBoxPoint> BoundingBoxPoints { get; set; }
    }
}
