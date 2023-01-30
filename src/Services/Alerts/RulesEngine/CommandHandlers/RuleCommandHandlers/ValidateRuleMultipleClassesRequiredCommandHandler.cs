using MediatR;
using RulesEngine.Commands.RuleCommands;

namespace RulesEngine.CommandHandlers.RulesCommandHandler
{
    public class ValidateRuleMultipleClassesRequiredCommandHandler : IRequestHandler<ValidateRuleMultipleClassesRequiredCommand, bool>
    {
        public async Task<bool> Handle(ValidateRuleMultipleClassesRequiredCommand request, CancellationToken cancellationToken)
        {
            foreach(var @class in request.RuleConfig.MultipleObjects)
            {
                var classesMatched = request.FoundClasses.Where(c => c.EventType == @class);
                classesMatched.ToList().ForEach(c => request.MatchingClassesBoxes.AddRange(c.BoundingBoxes));
                if(classesMatched.Count() == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
