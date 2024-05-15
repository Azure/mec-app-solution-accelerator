using MediatR;
using RulesEngine.Events;
using RulesEngine.Events.Base;

namespace RulesEngine.Commands
{
    public class AnalyzeObjectDetectionCommand : IRequest
    {
        public string Id { get; set; }
        public string UrlVideoEncoded { get; set; }
        public string Frame { get; set; }
        public string DetectionName { get; set; }
        public string SourceId { get; set; }
        public long EveryTime { get; set; }
        public string DetectionType { get; set; }
        public List<DetectionClass> Classes { get; set; }
        public List<StepTime> TimeTrace { get; set; }
    }
}
