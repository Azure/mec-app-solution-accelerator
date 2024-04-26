using Alerts.API.Configuration;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.Events;
using MQTTnet;
using MQTTnet.Client;
using SolTechnology.Avro;

namespace AlertsReader.Services
{
    public class AlertsSubscriptionWorker : BackgroundService
    {
        private readonly ILogger<AlertsSubscriptionWorker> _logger;
        private readonly IMediator _mediator;
        private readonly MqttConfig mqttConfig;

        public AlertsSubscriptionWorker(ILogger<AlertsSubscriptionWorker> logger, IMediator mediator, IOptions<MqttConfig> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            mqttConfig = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var mqttFactory = new MqttFactory();

            using var mqttClient = mqttFactory.CreateMqttClient();
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(mqttConfig.ConnectionString, mqttConfig.Port).Build();

            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                Console.WriteLine("### RECEIVED ALERT READER MESSAGE ###");
                var now = DateTime.UtcNow;
                var detection = AvroConvert.Deserialize<DetectedObjectAlert>(e.ApplicationMessage.PayloadSegment);
                detection.TimeTrace.FirstOrDefault(t => t.StepName.Equals("RulesEngine")).StepEnd = (long)(now - new DateTime(1970, 1, 1)).TotalMilliseconds;

                var alert = await _mediator.Send(new PersistAlertCommand()
                {
                    Information = detection.Information,
                    CaptureTime = detection.EveryTime,
                    Type = detection.Type,
                    Frame = detection.Frame,
                    Accuracy = detection.Accuracy,
                    StepTrace = detection.TimeTrace,
                    MatchingClasses = detection.MatchingClasses,
                    Source = detection.SourceId
                });

                _logger.LogInformation("Stored generic alert");
            };

            mqttClient.ConnectedAsync += e =>
            {
                Console.WriteLine("Connected");
                return Task.CompletedTask;
            };

            var test = await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
            Console.WriteLine(test.ResultCode);

            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("newAlert");
                        f.WithAtLeastOnceQoS();
                    })
                .Build();
            var subResult = await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
            Console.WriteLine(subResult.ReasonString);
            Console.WriteLine("MQTT client subscribed to topic.");

            while (true)
            {
            }
        }
    }
}
