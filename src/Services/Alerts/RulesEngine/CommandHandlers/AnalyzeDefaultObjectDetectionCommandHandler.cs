using Dapr.Client;
using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;
using SolTechnology.Avro;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.CommandHandlers
{
    public class AnalyzeDefaultObjectDetectionCommandHandler : IRequestHandler<AnalyzeDefaultObjectDetection>
    {
        private readonly DaprClient _daprClient;

        public AnalyzeDefaultObjectDetectionCommandHandler(DaprClient daprClient)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        }

        public async Task<Unit> Handle(AnalyzeDefaultObjectDetection request, CancellationToken cancellationToken)
        {
            var alert = new DetectedObjectAlert()
            {
                EventName = request.EventName,
                AlertTriggerTimeIni = new DateTime(request.EveryTime),
                Information = request.Information,
                Frames = new List<DetectionFrame>(),
                EventType = request.EventType,
            };

            var serialized = AvroConvert.Serialize(alert);
            await _daprClient.PublishEventAsync("pubsub", "newAlert", serialized);

            return Unit.Value;
        }
    }
}
