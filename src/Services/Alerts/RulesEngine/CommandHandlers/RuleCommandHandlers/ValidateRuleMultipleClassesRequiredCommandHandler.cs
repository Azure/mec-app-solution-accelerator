using MediatR;
using RulesEngine.Commands.RuleCommands;

namespace RulesEngine.CommandHandlers.RulesCommandHandler
{
    public class ValidateRuleMultipleClassesRequiredCommandHandler : IRequestHandler<ValidateRuleMultipleClassesRequiredCommand, bool>
    {
        public async Task<bool> Handle(ValidateRuleMultipleClassesRequiredCommand request, CancellationToken cancellationToken)
        {
            return !request.RuleConfig.MultipleObjects.Except(request.FoundClasses).Any();
        }
    }
}
