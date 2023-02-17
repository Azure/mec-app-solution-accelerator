using Microsoft.MecSolutionAccelerator.Services.Alerts.Events.Base;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts
{
    public class DetectionClass
    {
        public string EventType { get; set; }
        public float Confidence { get; set; }
        public List<BoundingBox> BoundingBoxes { get; set; }
    }
}
