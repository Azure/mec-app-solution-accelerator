using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.CommandHandlers;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;
using Dapr.Client;
using MediatR;
using RulesEngine.Commands.RuleCommands;
using RulesEngine.CommandHandlers.RulesCommandHandler;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Test
{
    public class RulesEngineTests
    {
        [Fact]
        public async Task RulesEngineHandlerWithVoidClassesFailsCorrectly()
        {

            //arrange pending add to fixture.
            var mockDaprClient = new Mock<DaprClient>();

            var alertsByDetectedClasses = new Dictionary<string, IEnumerable<AlertsConfig>>();
            var commandsTypeByDetectionName = new Dictionary<string, Type>();
            var mockMediator = new Mock<IMediator>();

            mockDaprClient.Setup(d => d.PublishEventAsync<byte[]>("pubsub",
                    "alerts",
                    It.IsAny<byte[]>(),
                    It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var commandHandler = new AnalyzeObjectDetectionCommandHandler(mockDaprClient.Object, alertsByDetectedClasses, mockMediator.Object);

            //assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await commandHandler.Handle(new AnalyzeObjectDetectionCommand(), CancellationToken.None));
        }

        [Fact]
        public async Task TestWithClassesWorksCorrectlyNotAlertTriggered()
        {
            //arrange
            var mockDaprClient = new Mock<DaprClient>();

            var alertsByDetectedClasses = new Dictionary<string, IEnumerable<AlertsConfig>>();
            var commandsTypeByDetectionName = new Dictionary<string, Type>();
            var mockMediator = new Mock<IMediator>();

            mockDaprClient.Setup(d => d.PublishEventAsync<byte[]>("pubsub",
                    "alerts",
                    It.IsAny<byte[]>(),
                    It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var commandHandler = new AnalyzeObjectDetectionCommandHandler(mockDaprClient.Object, alertsByDetectedClasses, mockMediator.Object);

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

        public async Task TestWithClassesWorksCorrectlyAlertTriggered()
        {
            //arrange
            var mockDaprClient = new Mock<DaprClient>();

            var alertsByDetectedClasses = new Dictionary<string, IEnumerable<AlertsConfig>>();
            alertsByDetectedClasses.Add("person", new List<AlertsConfig>() 
            {
                new AlertsConfig
                {
                    AlertName = "MinimumThresholdValidation",
                    RulesConfig = new List<RulesConfig>()
                    {            
                        new RulesConfig()
                        {
                            DetectedObject = "person",
                            MinimumThreshold = 75
                        }
                    },
                }
            }) ;

            var commandsTypeByDetectionName = new Dictionary<string, Type>();
            commandsTypeByDetectionName.Add("MinimumThresholdValidation", typeof(ValidateRuleMinimumThresholdRequiredCommand));

            var mockMediator = new Mock<IMediator>();

            mockDaprClient.Setup(d => d.PublishEventAsync<byte[]>("pubsub",
                    "alerts",
                    It.IsAny<byte[]>(),
                    It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

            var commandHandler = new AnalyzeObjectDetectionCommandHandler(mockDaprClient.Object, alertsByDetectedClasses, mockMediator.Object);

            //act
            var task = await commandHandler.Handle(new AnalyzeObjectDetectionCommand()
            {
                
                Classes = new List<DetectionClass>()
                {
                    new DetectionClass()
                    {
                        EventType = "person",
                        Confidence = 80,
                    }
                },
            }, CancellationToken.None);

            //assert
            Assert.IsType<Unit>(task);
        }

        [Fact]
        public async Task CheckMinimumNumberofObjectsValidationWorks()
        {
            //arrange
            var handler = new ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommandHandler();
            var command = new ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommand()
            {
                FoundClasses = new List<DetectionClass> { 
                    new DetectionClass() { EventType = "person", BoundingBoxes = new List<BoundingBox>(), Confidence = 90, }, 
                    new DetectionClass() { EventType = "person", BoundingBoxes = new List<BoundingBox>(), Confidence = 90, }, 
                    new DetectionClass() { EventType = "person", BoundingBoxes = new List<BoundingBox>(), Confidence = 90, } 
                },
                RuleConfig = new RulesConfig()
                {
                    NumberfObjects = 3,
                    DetectedObject = "person",
                },
                MatchedClassesByAlert = new List<DetectionClass>(),
            };

            //act
            var task = await handler.Handle(command, CancellationToken.None);

            //assert
            Assert.True(task);
        }

        [Fact]
        public async Task CheckMinimumNumberofObjectsValidationDontWorksCorrectly()
        {
            //arrange
            var handler = new ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommandHandler();
            var command = new ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommand()
            {
                FoundClasses = new List<DetectionClass> { 
                    new DetectionClass() { EventType = "person", BoundingBoxes = new List<BoundingBox>(), Confidence = 90, }, 
                    new DetectionClass() { EventType = "person", BoundingBoxes = new List<BoundingBox>(), Confidence = 90, } },
                RuleConfig = new RulesConfig()
                {
                    NumberfObjects = 3,
                    DetectedObject = "person",
                },
                MatchedClassesByAlert = new List<DetectionClass>(),
            };

            //act
            var task = await handler.Handle(command, CancellationToken.None);

            //assert
            Assert.False(task);
        }

        [Fact]
        public async Task CheckMinimuThresholdValidationWorks()
        {
            //arrange
            var handler = new ValidateRuleMinimumThresholdRequiredCommandHandler();
            var command = new ValidateRuleMinimumThresholdRequiredCommand()
            {
                RequestClass = new DetectionClass() 
                { 
                    EventType = "person", 
                    Confidence = 70, 
                    BoundingBoxes = new List<BoundingBox>(),
                },
                RuleConfig = new RulesConfig()
                {
                    DetectedObject = "person",
                    MinimumThreshold = 70,
                },
                MatchedClassesByAlert = new List<DetectionClass>(),
            };

            //act
            var task = await handler.Handle(command, CancellationToken.None);

            //assert
            Assert.True(task);
        }

        [Fact]
        public async Task CheckMinimuThresholdValidationDontWorksCorrectly()
        {
            //arrange
            var handler = new ValidateRuleMinimumThresholdRequiredCommandHandler();
            var command = new ValidateRuleMinimumThresholdRequiredCommand()
            {
                RequestClass = new DetectionClass()
                {
                    EventType = "person",
                    Confidence = 0.65F,
                    BoundingBoxes = new List<BoundingBox>(),
                },
                RuleConfig = new RulesConfig()
                {
                    DetectedObject = "person",
                    MinimumThreshold = 70,
                },
                MatchedClassesByAlert = new List<DetectionClass>(),
            };

            //act
            var task = await handler.Handle(command, CancellationToken.None);

            //assert
            Assert.False(task);
        }

        [Fact]
        public async Task CheckMinimuMultipleClassesValidationWorks()
        {
            //arrange
            var handler = new ValidateRuleMultipleClassesRequiredCommandHandler();
            var command = new ValidateRuleMultipleClassesRequiredCommand()
            {
                FoundClasses = new List<DetectionClass> { 
                    new DetectionClass() { EventType = "person", BoundingBoxes  = new List<BoundingBox>(), Confidence = 90, } , 
                    new DetectionClass() { EventType = "car", BoundingBoxes = new List<BoundingBox>(), Confidence = 90, } },
                MatchedClassesByAlert = new List<DetectionClass>(),
                RequestClass = new DetectionClass()
                {
                    EventType = "person",
                },
                RuleConfig = new RulesConfig()
                {
                    DetectedObject = "person",
                    MultipleObjects = new List<string> { "person", "car" }
                }
            };

            //act
            var task = await handler.Handle(command, CancellationToken.None);

            //assert
            Assert.True(task);
        }

        [Fact]
        public async Task CheckMinimuMultipleClassesValidationDontWorksCorrectly()
        {
            //arrange
            var handler = new ValidateRuleMultipleClassesRequiredCommandHandler();
            var command = new ValidateRuleMultipleClassesRequiredCommand()
            {
                FoundClasses = new List<DetectionClass> { 
                    new DetectionClass() { EventType = "car", BoundingBoxes = new List<BoundingBox>(), Confidence = 90, }, 
                },
                RequestClass = new DetectionClass()
                {
                    EventType = "person",
                },
                RuleConfig = new RulesConfig()
                {
                    DetectedObject = "person",
                    MultipleObjects = new List<string> { "person", "car" }
                }
            };
            command.MatchedClassesByAlert = new List<DetectionClass>();
            //act
            var task = await handler.Handle(command, CancellationToken.None);

            //assert
            Assert.False(task);
        }
    }
}