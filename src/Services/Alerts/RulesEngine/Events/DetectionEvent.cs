using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events
{
    public class DetectionEvent : BaseEvent
    {
        public string Frame { get; set; }
        public string Type { get; set; }
        public string UrlVideoEncoded { get; set; }
        public List<DetectionClass> Classes { get; set; }
        public List<StepTime> time_trace { get; set; }
    }
}
