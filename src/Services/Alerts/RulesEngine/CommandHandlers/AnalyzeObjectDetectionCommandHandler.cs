using Alerts.RulesEngine.Commands;
using Dapr.Client;
using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;
using Newtonsoft.Json;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.CommandHandlers
{
    public class AnalyzeObjectDetectionCommandHandler : IRequestHandler<AnalyzeObjectDetectionCommand, Unit>
    {
        private readonly DaprClient _daprClient;
        private readonly Dictionary<string, IEnumerable<AlertsConfig>> _alertsByDetectedClasses;
        private readonly IMediator _mediator;

        public AnalyzeObjectDetectionCommandHandler(DaprClient daprClient, Dictionary<string, IEnumerable<AlertsConfig>> alertsByDetectedClasses, IMediator mediator)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
            _alertsByDetectedClasses = alertsByDetectedClasses ?? throw new ArgumentNullException(nameof(alertsByDetectedClasses));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Unit> Handle(AnalyzeObjectDetectionCommand command, CancellationToken cancellationToken)
        {
            var stepTime = new StepTime { StepName = "RuleEngine", StepStart = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds };

            if (command.Classes is null || command.Classes.Count == 0)
            {
                throw new ArgumentException("Classes are required", nameof(command.Classes));
            }

            Task[] tasks = new Task[command.Classes.Count];
            var i = 0;
            foreach (var @class in command.Classes)
            {
                tasks[i++] = ValidateAlertsPerDetection(@class, command.Classes, command.EveryTime, command.UrlVideoEncoded,  command.Frame, command.TimeTrace, stepTime);

            }
            await Task.WhenAll(tasks);

            return Unit.Value;
         }

        private async Task ValidateAlertsPerDetection(DetectionClass requestClass, List<DetectionClass> foundClasses, long everyTime, string urlEncoded, string frame, List<StepTime> stepTrace, StepTime stepTime)
        {
            if (_alertsByDetectedClasses.TryGetValue(requestClass.EventType, out IEnumerable<AlertsConfig>? alertsConfig))
            {
                foreach (var alertConfig in alertsConfig)
                {
                    var validationAlertCommand = new ValidateAlertCommand()
                    {
                        EveryTime = everyTime,
                        UrlEncoded = urlEncoded,
                        AlertConfig = alertConfig,
                        FoundClasses = foundClasses,
                        Frame = frame,
                        RequestClass = requestClass,
                        StepTime = stepTime,
                        StepTrace = stepTrace,
                    };

                    await this._mediator.Send(validationAlertCommand);
                }
            }
        }
    }
}
