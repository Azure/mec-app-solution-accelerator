using MediatR;
using RulesEngine.Commands.RuleCommands;

namespace RulesEngine.CommandHandlers.RulesCommandHandler
{
    public class ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommandHandler : IRequestHandler<ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommand, bool>
    {
        public async Task<bool> Handle(ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommand request, CancellationToken cancellationToken)
        {
            var count = 0;
            foreach(var f in request.FoundClasses)
            {
                if(f.EventType == request.RuleConfig.DetectedObject)
                {
                    request.MatchingClassesBoxes.AddRange(f.BoundingBoxes);
                    count++;
                }
            }
            return count >= request.RuleConfig.NumberfObjects;
        }
    }
}
