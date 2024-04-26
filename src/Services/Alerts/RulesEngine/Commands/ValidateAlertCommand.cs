using MediatR;
using RulesEngine.Configuration;
using RulesEngine.Events;
using RulesEngine.Events.Base;

namespace RulesEngine.Commands
{
    public class ValidateAlertCommand : IRequest
    {
        public DetectionClass RequestClass { get; set; }
        public List<DetectionClass> FoundClasses { get; set; }
        public long EveryTime { get; set; }
        public string UrlEncoded { get; set; }
        public string Frame { get; set; }
        public List<StepTime> StepTrace { get; set; }
        public AlertsConfig AlertConfig { get; set; }
        public string SourceId { get; set; }
    }
}
