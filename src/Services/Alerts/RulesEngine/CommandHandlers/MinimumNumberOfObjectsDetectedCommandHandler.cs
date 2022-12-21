using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;

namespace RulesEngine.CommandHandlers
{
    public class MinimumNumberOfObjectsDetectedCommandHandler : IRequestHandler<MinimumNumberOfObjectsDetectedCommand, bool>
    {
        public async Task<bool> Handle(MinimumNumberOfObjectsDetectedCommand request, CancellationToken cancellationToken)
        {
           return request.FoundClasses.Where(foundClass => foundClass == request.RuleConfig.DetectedObject).Count() >= request.RuleConfig.NumberfObjects;
        }
    }
}
