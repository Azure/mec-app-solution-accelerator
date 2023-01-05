using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Commands
{
    public class PersistAlertCommand : IRequest<Guid>
    {
        public PersistAlertCommand()
        {
        }

        public string Information { get; set; }
        public string UrlVideoEncoded { get; set; }
        public string Frame { get; set; }
        public DateTime AlertTriggerTimeIni { get; set; }
        public DateTime AlertTriggerTimeFin { get; set; }
        public string Type { get; set; }
    }
}
