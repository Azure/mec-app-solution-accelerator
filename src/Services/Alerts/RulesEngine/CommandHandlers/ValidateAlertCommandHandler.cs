using Alerts.RulesEngine.Commands;
using Dapr.Client;
using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events;
using SolTechnology.Avro;

namespace Alerts.RulesEngine.CommandHandlers
{
    public class ValidateAlertCommandHandler : IRequestHandler<ValidateAlertCommand, Unit>
    {
        private readonly Dictionary<string, Type> _commandsTypeByDetectionName;
        private readonly IMediator _mediator;
        private readonly DaprClient _daprClient;

        public ValidateAlertCommandHandler(Dictionary<string, Type> commandsTypeByDetectionName, IMediator mediator, DaprClient daprClient)
        {
            _commandsTypeByDetectionName = commandsTypeByDetectionName ?? throw new ArgumentNullException(nameof(commandsTypeByDetectionName));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        }

        public async Task<Unit> Handle(ValidateAlertCommand request, CancellationToken cancellationToken)
        {
        var matchingClasses = new List<DetectionClass>();
            if (await ValidateAllRulesPerAlert(request.AlertConfig, request.RequestClass, request.FoundClasses, matchingClasses))
            {
                var alert = new DetectedObjectAlert()
                {
                    Name = request.AlertConfig.AlertName,
                    EveryTime = request.EveryTime,
                    UrlVideoEncoded = request.UrlEncoded,
                    Frame = request.Frame,
                    MatchingClasses = matchingClasses,
                    Type = request.AlertConfig.AlertName,
                    Information = $"Generate alert {request.AlertConfig.AlertName} detecting objects {string.Join(" ,", request.FoundClasses.Select(x => x.EventType).ToArray())}",
                    Accuracy = request.RequestClass.Confidence,
                    TimeTrace = request.StepTrace,
                };

                var serialized = AvroConvert.Serialize(alert);
                await _daprClient.PublishEventAsync("pubsub", "newAlert", serialized);
            }

            return Unit.Value;
        }

        private async ValueTask<bool> ValidateAllRulesPerAlert(AlertsConfig config, DetectionClass requestClass, List<DetectionClass> foundClasses, List<DetectionClass> matchingClasses)
        {
            Task<bool>[] tasks = new Task<bool>[config.RulesConfig.Count];
            bool exists = false;
            var i = 0;

            foreach (var ruleConfig in config.RulesConfig)
            {
                if (_commandsTypeByDetectionName.TryGetValue(ruleConfig.RuleName, out Type? eventType) && eventType != null)
                {
                    dynamic command = Activator.CreateInstance(eventType);
                    command.FoundClasses = foundClasses;
                    command.RequestClass = requestClass;
                    command.RuleConfig = ruleConfig;
                    command.MatchedClassesByAlert = matchingClasses;
                    tasks[i++] = _mediator.Send(command);
                    exists = true;
                }
            }

            await Task.WhenAll(tasks);

            return exists && tasks.All(task => task.IsCompletedSuccessfully && task.Result);
        }

    }
}
