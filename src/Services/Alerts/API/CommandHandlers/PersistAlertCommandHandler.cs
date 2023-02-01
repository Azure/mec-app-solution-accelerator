using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.CommandHandlers
{
    public class PersistAlertHandler : IRequestHandler<PersistAlertCommand, Guid>
    {
        private readonly IAlertsRepository _repository;

        public PersistAlertHandler(IAlertsRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Guid> Handle(PersistAlertCommand request, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            TimeSpan time = TimeSpan.FromMilliseconds(request.CaptureTime);
            DateTime captureDate = new DateTime(1970, 1, 1) + time;
            DateTime alertDate = DateTime.UtcNow;

            var entity = new Alert()
            {
                Frame = request.Frame,
                CaptureTime = captureDate,
                AlertTime = alertDate,
                Information = request.Information,
                MsExecutionTime = (alertDate - captureDate).TotalMilliseconds,
                Id = id,
                Type = request.Type,
                Accuracy = request.Accuracy * 100,
                StepTimes = this.SetDurations(request.StepTrace),
            };
            if (entity.Source == null)
            {
                entity.Source = this.SetHardwareMockInformation();
            }
            await this._repository.Create(entity);
            return id;
        }

        private IEnumerable<StepTime> SetDurations(List<StepTime> stepTrace)
        {
            stepTrace.ForEach(stepTime => stepTime.StepDuration = (stepTime.StepStop - stepTime.StepStart).TotalMilliseconds);
            return stepTrace;
        }

        private Source SetHardwareMockInformation()
        {
            var randomGenerator = new Random();
            var randomCameraNumber = randomGenerator.Next(1, 10);
            var source = new Source();
            source.Name = $"Camera {randomCameraNumber}";
            source.lat = randomGenerator.Next(1, 100);
            source.@lon = randomGenerator.Next(1, 10);
            source.Type = "Camera";

            return source;
        }
    }
}
