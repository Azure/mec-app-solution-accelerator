using Microsoft.MecSolutionAccelerator.Services.Alerts.Events.Base;


namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Events
{
    public class DetectedObjectAlert : BaseEvent
    {
        public string Frame { get; set; }
        public string Type { get; set; }
        public string UrlVideoEncoded { get; set; }
        public List<BoundingBoxPoint> BoundingBoxes { get; set; }
        public float Accuracy { get; set; }
        public List<StepTime> TimeTrace { get; set; }
    }
}
