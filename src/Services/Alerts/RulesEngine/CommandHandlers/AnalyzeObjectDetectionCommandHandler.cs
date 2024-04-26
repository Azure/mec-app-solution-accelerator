using MediatR;
using RulesEngine.Commands;
using RulesEngine.Configuration;
using RulesEngine.Events;
using RulesEngine.Events.Base;

namespace RulesEngine.CommandHandlers
{
    public class AnalyzeObjectDetectionCommandHandler : IRequestHandler<AnalyzeObjectDetectionCommand, Unit>
    {
        private readonly Dictionary<string, IEnumerable<AlertsConfig>> _alertsByDetectedClasses;
        private readonly IMediator _mediator;

        public AnalyzeObjectDetectionCommandHandler(Dictionary<string, IEnumerable<AlertsConfig>> alertsByDetectedClasses, IMediator mediator)
        {
            _alertsByDetectedClasses = alertsByDetectedClasses ?? throw new ArgumentNullException(nameof(alertsByDetectedClasses));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Unit> Handle(AnalyzeObjectDetectionCommand command, CancellationToken cancellationToken)
        {
            Console.WriteLine("ANALYZEOBJECTDETECTIONHANDLER");
            if (command.Classes is null || command.Classes.Count == 0)
            {
                throw new ArgumentException("Classes are required", nameof(command.Classes));
            }

            Task[] tasks = new Task[command.Classes.Count];
            var i = 0;
            foreach (var @class in command.Classes)
            {
                tasks[i++] = ValidateAlertsPerDetection(@class, command.Classes, command.EveryTime, command.UrlVideoEncoded, command.Frame, command.TimeTrace, command.SourceId);

            }
            await Task.WhenAll(tasks);

            return Unit.Value;
        }

        private async Task ValidateAlertsPerDetection(DetectionClass requestClass, List<DetectionClass> foundClasses, long everyTime, string urlEncoded, string frame, List<StepTime> stepTrace, string sourceId)
        {
            Console.WriteLine("VALIDATINGALERTDETECTION");
            if (_alertsByDetectedClasses.TryGetValue(requestClass.EventType, out IEnumerable<AlertsConfig> alertsConfig))
            {
                Console.WriteLine("FOUND ALERTS " + alertsConfig.Count());
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
                        StepTrace = stepTrace,
                        SourceId = sourceId,
                    };

                    await _mediator.Send(validationAlertCommand);
                }
            }
        }
    }
}
