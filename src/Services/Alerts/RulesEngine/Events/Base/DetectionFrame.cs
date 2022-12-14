namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base
{
    public class DetectionFrame
    {
        public string Frame { get; set; }
        public string Type { get; set; }
        public string UrlVideoEncoded { get; set; }
        public List<BoundingBox> BoundingBoxes { get; set; }
    }
}
