using MediatR;
using RulesEngine.Configuration;
using RulesEngine.Events.Base;
using RulesEngine.InMemoryDataDI;

namespace RulesEngine.Commands.RuleCommands
{
    [RuleTag("MultipleClassesDetected")]
    public class ValidateRuleMultipleClassesRequiredCommand : IRequest<bool>
    {
        public RulesConfig RuleConfig { get; set; }
        public DetectionClass RequestClass { get; set; }
        public long EveryTime { get; set; }
        public List<DetectionClass> FoundClasses { get; set; }
        public List<DetectionClass> MatchedClassesByAlert { get; set; }
    }
}
