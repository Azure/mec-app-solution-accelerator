using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.CommandHandlers
{
    public class MultipleClassesDetectedValidationCommandHandler : IRequestHandler<MultipleClassesDetectedValidationCommand, bool>
    {
        public async Task<bool>Handle(MultipleClassesDetectedValidationCommand request, CancellationToken cancellationToken)
        {
           return !request.RuleConfig.MultipleObjects.Except(request.FoundClasses).Any();
        }
    }
}
