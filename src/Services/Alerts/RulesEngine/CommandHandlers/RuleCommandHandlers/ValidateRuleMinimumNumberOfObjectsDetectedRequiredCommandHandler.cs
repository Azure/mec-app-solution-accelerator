using MediatR;
using RulesEngine.Commands.RuleCommands;

namespace RulesEngine.CommandHandlers.RulesCommandHandler
{
    public class ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommandHandler : IRequestHandler<ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommand, bool>
    {
        public async Task<bool> Handle(ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommand request, CancellationToken cancellationToken)
        {
            var count = 0;
            foreach(var foundClass in request.FoundClasses)
            {
                if(foundClass.EventType == request.RuleConfig.DetectedObject)
                {
                    request.MatchedClassesByAlert.Add(foundClass);
                    count++;
                }
            }
            return count >= request.RuleConfig.NumberfObjects;
        }
    }
}
