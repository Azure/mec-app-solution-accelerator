using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.CommandHandlers;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Configuration;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;
using Dapr.Client;
using MediatR;
using RulesEngine.Commands.RuleCommands;
using RulesEngine.CommandHandlers.RulesCommandHandler;
using Microsoft.Extensions.Logging;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.EventControllers;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Test
{
    public class RulesEngineTests
    {
        [Fact]
        public async Task RulesEngineHandlerWithVoidClassesFailsCorrectly()
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
        public async Task TestWithClassesWorksCorrectlyNotAlertTriggered()
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
            Assert.False(task);
        }

        public async Task TestWithClassesWorksCorrectlyAlertTriggered()
        {
            //arrange
            var mockDaprClient = new Mock<DaprClient>();

            var alertsByDetectedClasses = new Dictionary<string, List<AlertsConfig>>();
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

            var commandHandler = new AnalyzeObjectDetectionCommandHandler(mockDaprClient.Object, alertsByDetectedClasses, commandsTypeByDetectionName, mockMediator.Object);

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
            Assert.True(task);
        }

        //[Fact]
        //public async Task CheckMinimumNumberofObjectsValidationWorks()
        //{
        //    //arrange
        //    var handler = new ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommandHandler();
        //    var command = new ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommand()
        //    {
        //        FoundClasses = new List<string> {"person", "person", "person" },
        //        RuleConfig = new RulesConfig()
        //        {
        //            NumberfObjects = 3,
        //            DetectedObject = "person",
        //        }
        //    };

        //    //act
        //    var task = await handler.Handle(command, CancellationToken.None);

        //    //assert
        //     Assert.True(task);
        //}

        //[Fact]
        //public async Task CheckMinimumNumberofObjectsValidationDontWorksCorrectly()
        //{
        //    //arrange
        //    var handler = new ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommandHandler();
        //    var command = new ValidateRuleMinimumNumberOfObjectsDetectedRequiredCommand()
        //    {
        //        FoundClasses = new List<string> { "person", "person"},
        //        RuleConfig = new RulesConfig()
        //        {
        //            NumberfObjects = 3,
        //            DetectedObject = "person",
        //        }
        //    };

        //    //act
        //    var task = await handler.Handle(command, CancellationToken.None);

        //    //assert
        //    Assert.False(task);
        //}

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
                },
                RuleConfig = new RulesConfig()
                {
                    DetectedObject = "person",
                    MinimumThreshold = 70,
                }
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
                },
                RuleConfig = new RulesConfig()
                {
                    DetectedObject = "person",
                    MinimumThreshold = 70,
                }
            };

            //act
            var task = await handler.Handle(command, CancellationToken.None);

            //assert
            Assert.False(task);
        }

        //[Fact]
        //public async Task CheckMinimuMultipleClassesValidationWorks()
        //{
        //    //arrange
        //    var handler = new ValidateRuleMultipleClassesRequiredCommandHandler();
        //    var command = new ValidateRuleMultipleClassesRequiredCommand()
        //    {
        //        FoundClasses = new List<string> { "person", "car" },
        //        RequestClass = new DetectionClass()
        //        {
        //            EventType = "person",
        //        },
        //        RuleConfig = new RulesConfig()
        //        {
        //            DetectedObject = "person",
        //            MultipleObjects = new List<string> { "person", "car" }
        //        }
        //    };

        //    //act
        //    var task = await handler.Handle(command, CancellationToken.None);

        //    //assert
        //    Assert.True(task);
        //}

        //[Fact]
        //public async Task CheckMinimuMultipleClassesValidationDontWorksCorrectly()
        //{
        //    //arrange
        //    var handler = new ValidateRuleMultipleClassesRequiredCommandHandler();
        //    var command = new ValidateRuleMultipleClassesRequiredCommand()
        //    {
        //        FoundClasses = new List<string> { "car" },
        //        RequestClass = new DetectionClass()
        //        {
        //            EventType = "person",
        //        },
        //        RuleConfig = new RulesConfig()
        //        {
        //            DetectedObject = "person",
        //            MultipleObjects = new List<string> { "person", "car" }
        //        }
        //    };

        //    //act
        //    var task = await handler.Handle(command, CancellationToken.None);

        //    //assert
        //    Assert.False(task);
        //}

        [Fact]
        public async Task CheckDetectionEventHandlerWorksCorrectly()
        {
            //arrange
            var mockMediator = new Mock<IMediator>();
            mockMediator.Setup(m => m.Send(It.IsAny<AnalyzeObjectDetectionCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var mockLogger = new Mock<ILogger<DetectionsProcessEventController>>();
            var @object = "{\r\n\t\"Frame\": \"Test\",\r\n\t\"UrlVideoEncoded\": \"Test\",\r\n\t\"Type\": \"Person\",\r\n\t\"Classes\": [{\r\n\t\t\"EventType\": \"person\",\r\n\t\t\"Confidence\": 70,\r\n\t\t\"BoundngBoxes\": [{\r\n\t\t\t\"x\": 332.2,\r\n\t\t\t\"y\": -32.76\r\n\t\t}]\r\n\t}]\r\n}";
            var controller = new DetectionsProcessEventController(mockLogger.Object, mockMediator.Object);

            //act
            var task = await controller.DetectionEventHandler(@object);

            //assert
            Assert.True(task);
        }
    }
}