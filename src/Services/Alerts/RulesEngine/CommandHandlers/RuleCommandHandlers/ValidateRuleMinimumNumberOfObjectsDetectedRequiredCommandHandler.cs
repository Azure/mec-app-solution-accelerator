using MediatR;
using RulesEngine.Commands.RuleCommands;

namespace RulesEngine.CommandHandlers.RulesCommandHandler
{
    public class ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommandHandler : IRequestHandler<ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommand, bool>
    {
        public async Task<bool> Handle(ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommand request, CancellationToken cancellationToken)
        {
            return request.FoundClasses.Where(foundClass => foundClass == request.RuleConfig.DetectedObject).Count() >= request.RuleConfig.NumberfObjects;
        }
    }
}
