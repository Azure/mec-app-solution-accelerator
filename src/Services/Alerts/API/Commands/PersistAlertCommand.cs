using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Commands
{
    public class PersistAlertCommand : IRequest<Alert>
    {
        public PersistAlertCommand()
        {
        }

        public string Information { get; set; }
        public string Frame { get; set; }
        public long CaptureTime { get; set; }
        public string Type { get; set; }
        public float Accuracy { get; set; }
        public List<StepTime> StepTrace { get; set; }
    }
}
