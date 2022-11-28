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
        public int Accuracy { get; set; }
        public string UrlVideoEncoded { get; set; }
        public DateTime AlertTriggerTimeIni { get; set; }
        public double lat { get; set; }
        public double @long { get; set; }
        public DateTime AlertTriggerTimeFin { get; set; }
        public string Type { get; set; }
    }
}
