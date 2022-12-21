using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.CommandHandlers
{
    public class FramesValidationCommandHandler : IRequestHandler<FramesValidationCommand, bool>
    {
        public async Task<bool> Handle(FramesValidationCommand request, CancellationToken cancellationToken)
        {
            //remake completo en los proximos commits.
            return true;
        }
    }
}
