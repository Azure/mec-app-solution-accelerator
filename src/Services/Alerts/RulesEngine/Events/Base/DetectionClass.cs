using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;

namespace RulesEngine.Events.Base
{
    public class DetectionClass
    {
        public string EventType { get; set; }
        public float Confidence { get; set; }
        public List<BoundingBox> BoundingBoxes { get; set; }
    }
}
