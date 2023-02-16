using Alerts.RulesEngine.Commands;
using Dapr.Client;
using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events;
using SolTechnology.Avro;
using System.Text.Encodings.Web;

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
            var matchingClassesBoxes = new List<BoundingBox>();
            if (await ValidateAllRulesPerAlert(request.AlertConfig, request.RequestClass, request.FoundClasses, matchingClassesBoxes))
            {
                request.StepTime.StepEnd = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
                request.StepTrace.Add(request.StepTime);
                var alert = new DetectedObjectAlert()
                {
                    Name = request.AlertConfig.AlertName,
                    EveryTime = request.EveryTime,
                    UrlVideoEncoded = request.UrlEncoded,
                    Frame = request.Frame,
                    BoundingBoxes = matchingClassesBoxes,
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

        private async Task<bool> ValidateAllRulesPerAlert(AlertsConfig config, DetectionClass requestClass, List<DetectionClass> foundClasses, List<BoundingBox> matchingClassesBoxes)
        {
            var pendingTaks = new List<Task<bool>>();
            bool exists = default;
            foreach (var ruleConfig in config.RulesConfig)
            {
                exists = _commandsTypeByDetectionName.TryGetValue(requestClass.EventType, out Type eventType);
                if (exists)
                {
                    dynamic command = Activator.CreateInstance(eventType);
                    command.FoundClasses = foundClasses;
                    command.RequestClass = requestClass;
                    command.RuleConfig = ruleConfig;
                    command.MatchingClassesBoxes = matchingClassesBoxes;
                    pendingTaks.Add(_mediator.Send(command));
                }
            }
            await Task.WhenAll(pendingTaks);

            return exists ? pendingTaks
                .Where(task => task.Status == TaskStatus.RanToCompletion)
                .Select(x => x.Result)
                .ToList().All(result => result) : false;
        }
    }
}
