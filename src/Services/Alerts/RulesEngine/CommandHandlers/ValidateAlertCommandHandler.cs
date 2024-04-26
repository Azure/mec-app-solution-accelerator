using MediatR;
using MQTTnet;
using MQTTnet.Client;
using RulesEngine.Commands;
using RulesEngine.Commands.RuleCommands;
using RulesEngine.Configuration;
using RulesEngine.Events;
using RulesEngine.Events.Base;
using Microsoft.Extensions.Options;
using SolTechnology.Avro;

namespace RulesEngine.CommandHandlers
{
    public class ValidateAlertCommandHandler : IRequestHandler<ValidateAlertCommand, Unit>
    {
        private readonly Dictionary<string, Type> _commandsTypeByDetectionName;
        private readonly IMediator _mediator;
        private readonly MqttConfig _mqttConfig;

        public ValidateAlertCommandHandler(Dictionary<string, Type> commandsTypeByDetectionName, IMediator mediator, IOptions<MqttConfig> options)
        {
            _commandsTypeByDetectionName = commandsTypeByDetectionName ?? throw new ArgumentNullException(nameof(commandsTypeByDetectionName));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mqttConfig = options?.Value ?? throw new ArgumentNullException(nameof(options));
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
                    SourceId = request.SourceId
                };

                var serialized = AvroConvert.Serialize(alert);
                var mqttFactory = new MqttFactory();
                using var mqttClient = mqttFactory.CreateMqttClient();
                var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(_mqttConfig.ConnectionString, _mqttConfig.Port).Build();
                mqttClient.ConnectedAsync += e =>
                {
                    Console.WriteLine("Connected");
                    return Task.CompletedTask;
                };

                var test = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
                Console.WriteLine("Published new Alert result: " + test.ResultCode);

                var applicationMessage = new MqttApplicationMessageBuilder()
                    .WithTopic("newAlert")
                    .WithPayload(serialized)
                    .Build();

                await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
                await mqttClient.DisconnectAsync(cancellationToken: cancellationToken);
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
