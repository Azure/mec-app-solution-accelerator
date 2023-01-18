using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.CommandHandlers;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;
using Dapr.Client;
using MediatR;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Test
{
    public class RulesEngineTests
    {
        [Fact]
        public async Task RulesEngineHandler_With_Void_Classes_Fails_Correctly()
        {

            //arrange pending add to fixture.
            var mockDaprClient = new Mock<DaprClient>();

            var alertsByDetectedClasses = new Dictionary<string, List<AlertsConfig>>();
            var commandsTypeByDetectionName = new Dictionary<string, Type>();
            var mockMediator = new Mock<IMediator>();

            mockDaprClient.Setup(d => d.PublishEventAsync<byte[]>("pubsub",
                    "alerts",
                    It.IsAny<byte[]>(),
                    It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var commandHandler = new AnalyzeObjectDetectionCommandHandler(mockDaprClient.Object, alertsByDetectedClasses, commandsTypeByDetectionName, mockMediator.Object);

            //assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await commandHandler.Handle(new AnalyzeObjectDetectionCommand(), CancellationToken.None));
        }

        [Fact]
        public async Task Test_With_Classes_Works_Correctly()
        {
            //arrange
            var mockDaprClient = new Mock<DaprClient>();

            var alertsByDetectedClasses = new Dictionary<string, List<AlertsConfig>>();
            var commandsTypeByDetectionName = new Dictionary<string, Type>();
            var mockMediator = new Mock<IMediator>();

            mockDaprClient.Setup(d => d.PublishEventAsync<byte[]>("pubsub",
                    "alerts",
                    It.IsAny<byte[]>(),
                    It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var commandHandler = new AnalyzeObjectDetectionCommandHandler(mockDaprClient.Object, alertsByDetectedClasses, commandsTypeByDetectionName, mockMediator.Object);
            //act

            var task = await commandHandler.Handle(new AnalyzeObjectDetectionCommand()
            {
                Classes = new List<DetectionClass>() 
                { 
                    new DetectionClass() 
                    { 
                        EventType = "person",
                    } 
                },
            }, CancellationToken.None);

            //assert
            Assert.IsType<Unit>(task);
        }
    }
}