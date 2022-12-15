using Dapr.Client;
using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;
using SolTechnology.Avro;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.CommandHandlers
{
    public class AnalyzePlaneDetectionCommandHandler : IRequestHandler<AnalyzeBoatDetection>
    {
        private readonly DaprClient _daprClient;

        public AnalyzePlaneDetectionCommandHandler(DaprClient daprClient)
        {
            _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
        }

        public async Task<Unit> Handle(AnalyzeBoatDetection request, CancellationToken cancellationToken)
        {
            var alert = new DetectedBoatAlert()
            {
                EventName = request.EventName,
                AlertTriggerTimeIni = new DateTime(request.EveryTime),
                Information = request.Information,
                Frames = new List<DetectionFrame>(),
            };

            var serialized = AvroConvert.Serialize(alert);
            await _daprClient.PublishEventAsync("pubsub", "newBoatAlert", serialized);

            return Unit.Value;
        }
    }
}
