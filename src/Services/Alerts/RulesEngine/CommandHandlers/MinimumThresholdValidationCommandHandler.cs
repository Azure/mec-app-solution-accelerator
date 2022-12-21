using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.CommandHandlers
{
    public class MinimumThresholdValidationCommandHandler : IRequestHandler<MinimumThresholdValidationCommand, bool>
    {
        public async Task<bool> Handle(MinimumThresholdValidationCommand request, CancellationToken cancellationToken)
        {
            return request.RequestClass.Confidence > request.RuleConfig.MinimumThreshold;
        }
    }
}