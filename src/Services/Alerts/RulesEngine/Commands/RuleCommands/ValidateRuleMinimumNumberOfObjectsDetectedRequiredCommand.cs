using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Injection;
using RulesEngine.Events.Base;

namespace RulesEngine.Commands.RuleCommands
{
    [RuleTag("MinimumNumberOfObjectsDetected")]
    public class ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommand : IRequest<bool>
    {
        public RulesConfig RuleConfig { get; set; }
        public DetectionClass RequestClass { get; set; }
        public long EveryTime { get; set; }
        public List<string> FoundClasses { get; set; }
    }
}


