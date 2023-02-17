using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events 
{ 
    public class DetectedObjectAlert : BaseEvent
    {
        public string Frame { get; set; }
        public string Type { get; set; }
        public string UrlVideoEncoded { get; set; }
        public string AlertInformation { get; set; }
        public List<DetectionClass> MatchingClasses { get; set; }
        public float Accuracy { get; set; }
        public List<StepTime> TimeTrace { get; set; }
    }
}
