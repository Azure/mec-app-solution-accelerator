using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Injection;

namespace RulesEngine.Commands.RuleCommands
{
    [RuleTag("MinimumThresholdValidation")]
    public class ValidateRuleMinimumThresholdRequiredCommand : IRequest<bool>
    {
        public RulesConfig RuleConfig { get; set; }
        public DetectionClass RequestClass { get; set; }
        public long EveryTime { get; set; }
        public List<DetectionClass> FoundClasses { get; set; }
        public List<BoundingBox> MatchingClassesBoxes { get; set; }
    }
}
