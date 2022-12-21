using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Models;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.CommandHandlers
{
    public class FramesValidationCommandHandler : IRequestHandler<FramesValidationCommand, bool>
    {
        private readonly IDetectionsRepository _detectionsRepository;

        public FramesValidationCommandHandler(IDetectionsRepository detectionsRepository)
        {
            this._detectionsRepository = detectionsRepository ?? throw new ArgumentNullException(nameof(detectionsRepository));
        }

        public async Task<bool> Handle(FramesValidationCommand request, CancellationToken cancellationToken)
        {
            //var frames = await _detectionsRepository.GetFramesByClassNextInTime(request.EveryTime, request.RequestClass.EventType);
            //return frames.Count >= request.RuleConfig.MinimumNumberOfFrames ? true : false;
            //reviewing
            return true;
        }
    }
}
