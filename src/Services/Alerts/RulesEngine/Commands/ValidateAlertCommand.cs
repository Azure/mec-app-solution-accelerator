using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;

namespace Alerts.RulesEngine.Commands
{
    public class ValidateAlertCommand : IRequest
    {
        public DetectionClass RequestClass { get; set; }
        public List<DetectionClass> FoundClasses { get; set; }
        public long EveryTime { get; set; }
        public string UrlEncoded { get; set; }
        public string Frame { get; set; }
        public List<StepTime> StepTrace { get; set; }
        public StepTime StepTime { get; set; }
        public AlertsConfig AlertConfig { get; set; }
    }
}
