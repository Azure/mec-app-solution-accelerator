using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;
using Newtonsoft.Json;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.CommandHandlers
{
    public class PersistAlertHandler : IRequestHandler<PersistAlertCommand, Alert>
    {
        private readonly IAlertsRepository _repository;

        public PersistAlertHandler(IAlertsRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Alert> Handle(PersistAlertCommand request, CancellationToken cancellationToken)
        {
            var id = Guid.NewGuid();
            TimeSpan time = TimeSpan.FromMilliseconds(request.CaptureTime);
            DateTime captureDate = new DateTime(1970, 1, 1) + time;
            DateTime alertDate =  DateTime.Now;

            var entity = new Alert()
            {
                Frame = request.Frame,
                CaptureTime = captureDate,
                Information = request.Information,
                Id = id,
                Type = request.Type,
                Accuracy = request.Accuracy * 100,
                StepTimes2 = SetDurations(request.StepTrace),
                StepTimes = JsonConvert.SerializeObject(SetDurations(request.StepTrace)),
                MsExecutionTime = (alertDate - captureDate).TotalMilliseconds,
                AlertTime = alertDate,
                Source = this.SetHardwareMockInformation(),
                MatchesClasses = request.MatchingClasses,
            };

            await this._repository.Create(entity);
            return entity;
        }

        private IEnumerable<StepTimeAsDate> SetDurations(List<StepTime> stepTrace)
        {
            long previousEnd = 0;
            return stepTrace.Select(stepTraceItem =>
            {
                var contextualTraces = new StepTimeAsDate()
                {
                    StepStart = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(stepTraceItem.StepStart),
                    StepStop = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(stepTraceItem.StepEnd),
                    StepName = stepTraceItem.StepName,
                    StepDuration = previousEnd == 0 ? Math.Round(Convert.ToDecimal(stepTraceItem.StepEnd - stepTraceItem.StepStart)) : Math.Round(Convert.ToDecimal(stepTraceItem.StepEnd - previousEnd)),
                };
                previousEnd = stepTraceItem.StepEnd;
                return contextualTraces;
            }).ToList();
        }

        private Source SetHardwareMockInformation() //Mocking real hardware
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
