namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Events.Base
{
    public class Detection : BaseEvent
    {
        public string Frame { get; set; }
        public string Type { get; set; }
        public string UrlVideoEncoded { get; set; }
        public List<BoundingBoxPoint> BoundingBoxes { get; set; }
    }
}
