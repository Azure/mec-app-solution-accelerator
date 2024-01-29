namespace Microsoft.MecSolutionAccelerator.AlertsUI.Models
{
    public class AlertDetailsModel
    {
        

        public string Id { get; set; }
        public string Information { get; set; }
        public string Frame { get; set; }
        public DateTime CaptureTime { get; set; }
        public DateTime AlertTime { get; set; }
        public double MsExecutionTime { get; set; }
        public string Type { get; set; }
        public float Accuracy { get; set; }
        public SourceModel Source { get; set; }
        public IEnumerable<StepTimeModel> StepTimeAsDate { get; set; }

        public string toString()
        {
            return "Alert: " + Id + "\n SourceId: " + Source.Name + "\n Type: " + Type + "\n Information: " + Information + "\n Image: " + Frame + "\n Accuracy: " + Accuracy + "\n Initial time: " + CaptureTime;
        }
    }
}