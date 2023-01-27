using Dapr.Client;
using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;
using SolTechnology.Avro;
using System.Threading.Tasks;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.CommandHandlers
{
    public class AnalyzeObjectDetectionCommandHandler : IRequestHandler<AnalyzeObjectDetectionCommand, bool>
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

        public async Task<bool> Handle(AnalyzeObjectDetectionCommand command, CancellationToken cancellationToken)
        {
            if(command.Classes == null || command.Classes.Count == 0)
            {
                throw new ArgumentException("Classes are required");
            }

            var foundClasses = command.Classes.Select(x => x.EventType).ToList();
            var pendingTaks = new List<Task<bool>>();
            foreach(var @class in command.Classes)
            {
                pendingTaks.Add(
                    this.ValidateAlertsPerDetection( //Single class can generate multiple alerts
                        @class, 
                        foundClasses,
                        command.EveryTime,
                        command.UrlVideoEncoded,
                        command.Frame)
                    );
            }
            await Task.WhenAll(pendingTaks);
            var result = pendingTaks
                .Where(task => task.Status == TaskStatus.RanToCompletion)
                .Select(x => x.Result)
                .ToList().Any(x => x);

            return result;
        }

        private async Task<bool> ValidateAlertsPerDetection(DetectionClass requestClass, List<string> foundClasses, long everyTime, string urlEncoded, string frame)
        {
            var triggeredAlert = false;
            var exists = _alertsByDetectedClasses.TryGetValue(requestClass.EventType, out List<AlertsConfig> alertsConfig);
            if (exists)
            {
                foreach (var alertConfig in alertsConfig)
                {
                    var successfull = await ValidateAllRulesPerAlert(alertConfig, requestClass, foundClasses); //Validate all the required rules from the config, just and.
                    if (successfull)
                    {
                        triggeredAlert = successfull;
                        var alert = new DetectedObjectAlert()
                        {
                            Name = alertConfig.AlertName,
                            EveryTime = everyTime,
                            UrlVideoEncoded = urlEncoded,
                            Frame = frame,
                            BoundingBoxes = requestClass.BoundingBoxes,
                            Type = alertConfig.AlertName,
                            Information = $"Generate alert {alertConfig.AlertName} detecting objects {string.Join(",", foundClasses.ToArray())}",
                            Accuracy = requestClass.Confidence,
                        };

                        var serialized = AvroConvert.Serialize(alert);
                        await _daprClient.PublishEventAsync("pubsub", "newAlert", serialized);
                    }
                }
            }
            return triggeredAlert;
        }

        private async Task<bool> ValidateAllRulesPerAlert(AlertsConfig config, DetectionClass requestClass, List<string> foundClasses)
        {
            foreach (var ruleConfig in config.RulesConfig)
            {
                var eventType = _commandsTypeByDetectionName[ruleConfig.RuleName];

                dynamic command = Activator.CreateInstance(eventType);
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
