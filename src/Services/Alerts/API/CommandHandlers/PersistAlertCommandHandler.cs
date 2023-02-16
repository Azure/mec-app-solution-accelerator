﻿using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Models;
using Newtonsoft.Json;
using static Google.Rpc.Context.AttributeContext.Types;

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

            var entity = new Alert()
            {
                Frame = request.Frame,
                CaptureTime = captureDate,
                Information = request.Information,
                Id = id,
                Type = request.Type,
                Accuracy = request.Accuracy * 100,
                StepTimes = JsonConvert.SerializeObject(SetDurations(request.StepTrace)),
            };
            if (entity.Source == null)
            {
                entity.Source = this.SetHardwareMockInformation();
            }
            await this._repository.Create(entity);
            return entity;
        }

        private IEnumerable<StepTimeAsDate> SetDurations(List<StepTime> stepTrace)
        {
            var stepTimes = new List<StepTimeAsDate>();
            long previousEnd = 0;
            foreach(var stepTraceItem in stepTrace)
            {
                if(previousEnd == 0)
                {
                    var v = new StepTimeAsDate()
                    {
                        StepStart = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(stepTraceItem.StepStart),
                        StepStop = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(stepTraceItem.StepEnd),
                        StepName = stepTraceItem.StepName,
                        StepDuration = stepTraceItem.StepEnd - stepTraceItem.StepStart
                    };
                    stepTimes.Add(v);
                    previousEnd = stepTraceItem.StepEnd;
                }
                else
                {
                    var v = new StepTimeAsDate()
                    {
                        StepStart = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(stepTraceItem.StepStart),
                        StepStop = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(stepTraceItem.StepEnd),
                        StepName = stepTraceItem.StepName,
                    };
                    v.StepDuration = stepTraceItem.StepEnd - previousEnd;
                    stepTimes.Add(v);
                    previousEnd = stepTraceItem.StepEnd;
                }
            }

            stepTrace.ForEach(stepTime => stepTimes.Add(new StepTimeAsDate()
            { 
                StepStart = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(stepTime.StepStart),
                StepStop = new DateTime(1970, 1, 1) + TimeSpan.FromMilliseconds(stepTime.StepEnd),
                StepName = stepTime.StepName,
                StepDuration = stepTime.StepEnd - stepTime.StepStart 
            }));
            return stepTimes;
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