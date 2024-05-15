using MediatR;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using RulesEngine.Commands;
using RulesEngine.Configuration;
using RulesEngine.Events;
using SolTechnology.Avro;
using System.Text;

namespace RulesEngine;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IMediator mediator;
    private readonly MqttConfig mqttConfig;

    public Worker(ILogger<Worker> logger, IMediator mediator, IOptions<MqttConfig> options)
    {
        _logger = logger;
        this.mediator = mediator;
        mqttConfig = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mqttFactory = new MqttFactory();
        using var mqttClient = mqttFactory.CreateMqttClient();
        var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer(mqttConfig.ConnectionString, mqttConfig.Port).Build();
        mqttClient.ApplicationMessageReceivedAsync += async e =>
        {
            _logger.LogInformation("Received Rule Engine message");
            try
            {
                var stepTime = new StepTime { StepName = "RulesEngine", StepStart = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds };
                var detectionBytes = AvroConvert.Json2Avro(Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment));
                var detection = AvroConvert.Deserialize<ObjectDetected>(detectionBytes);
                detection.time_trace.Add(stepTime);

                var command = new AnalyzeObjectDetectionCommand()
                {
                    Id = detection.Id,
                    DetectionName = detection.Name,
                    EveryTime = detection.EveryTime,
                    Frame = detection.Frame,
                    DetectionType = detection.Type,
                    UrlVideoEncoded = detection.UrlVideoEncoded,
                    Classes = detection.Classes,
                    TimeTrace = detection.time_trace,
                    SourceId = detection.SourceId
                };
                await mediator.Send(command);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "An error occurred while processing the detection event");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the detection event");
            }
        };

        mqttClient.ConnectedAsync += e =>
        {
            _logger.LogInformation("Connected to MQTT broker");
            return Task.CompletedTask;
        };

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(
                f =>
                {
                    f.WithTopic("newDetection");
                    f.WithAtLeastOnceQoS();
                })
            .Build();

        var subResult = await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);
        _logger.LogInformation("MQTT client subscribed to topic with result ${0}", subResult.ReasonString);

        while (!stoppingToken.IsCancellationRequested)
        {
        }
    }
}