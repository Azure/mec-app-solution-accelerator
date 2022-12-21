using Dapr.Client;
using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events;
using RulesEngine.Events.Base;
using SolTechnology.Avro;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.CommandHandlers
{
    public class AnalyzeObjectDetectionCommandHandler : IRequestHandler<AnalyzeObjectDetectionCommand>
    {
        private readonly DaprClient _daprClient;
        private readonly Dictionary<string, List<AlertsConfig>> _alertsByDetectedClasses;
        private readonly Dictionary<string, Type> _commandsTypeByDetectionName;
        private readonly IMediator _mediator;

        public AnalyzeObjectDetectionCommandHandler(DaprClient daprClient, Dictionary<string, List<AlertsConfig>> alertsByDetectedClasses, Dictionary<string, Type> commandsTypeByDetectionName, IMediator mediator)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
            _alertsByDetectedClasses = alertsByDetectedClasses ?? throw new ArgumentNullException(nameof(alertsByDetectedClasses));
            _commandsTypeByDetectionName = commandsTypeByDetectionName ?? throw new ArgumentNullException(nameof(commandsTypeByDetectionName));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<Unit> Handle(AnalyzeObjectDetectionCommand request, CancellationToken cancellationToken)
        {
            var foundClasses = request.Classes.Select(x => x.EventType).ToList();
            var foundClassesDistinct = foundClasses.Distinct();
            var pendingTaks = new List<Task>();
            foreach(var foundClass in foundClassesDistinct)
            {
                pendingTaks.Add(this.ExecuteAlertsByClass(request.Classes.FirstOrDefault(obj => obj.EventType.Equals(foundClass)), foundClasses, request.EveryTime, request.Information, request.UrlVideoEncoded, request.Frame));
            }
            await Task.WhenAll(pendingTaks);

            return Unit.Value;
        }

        private async Task ExecuteAlertsByClass(DetectionClass requestClass, List<string> foundClasses, long everyTime, string information, string urlEncoded, string frame)
        {
            var exists =_alertsByDetectedClasses.TryGetValue(requestClass.EventType, out List<AlertsConfig> value);
            if (exists)
            {
                foreach (var alertConfig in value)
                {
                    var successfull = await ExecuteAlertsRules(alertConfig, requestClass, foundClasses, everyTime);
                    if (successfull)
                    {
                        var alert = new DetectedObjectAlert()
                        {
                            EventName = alertConfig.AlertName,
                            Information = information,
                            EveryTime = everyTime,
                            UrlVideoEncoded = urlEncoded,
                            Frame = frame,
                            BoundingBoxes = requestClass.BoundingBoxes,
                            Type = requestClass.EventType,
                        };

                        var serialized = AvroConvert.Serialize(alert);
                        await _daprClient.PublishEventAsync("pubsub", "newAlert", serialized);
                    }
                }
            }
        }

        private async Task<bool> ExecuteAlertsRules(AlertsConfig config, DetectionClass requestClass, List<string> foundClasses, long everyTime)
        {
            foreach (var ruleConfig in config.RulesConfig)
            {
                var eventType = _commandsTypeByDetectionName[ruleConfig.RuleName];

                dynamic command = Activator.CreateInstance(eventType);
                command.EveryTime = everyTime;
                command.FoundClasses = foundClasses;
                command.RequestClass = requestClass;
                command.RuleConfig = ruleConfig;
                var result = await _mediator.Send(command);
                if (!(bool)result)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
