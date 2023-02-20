namespace Microsoft.MecSolutionAccelerator.AlertsUI.Models
{
    public class AlertDetailsModel
    {
        public AlertDetailsModel(string id, string information, string frame, DateTime captureTime, DateTime alertTime, double msExecutionTime, string type, float accuracy, SourceModel source, string stepTimes)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Information = information ?? throw new ArgumentNullException(nameof(information));
            Frame = frame ?? throw new ArgumentNullException(nameof(frame));
            CaptureTime = captureTime;
            AlertTime = alertTime;
            MsExecutionTime = msExecutionTime;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Accuracy = accuracy;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            StepTimes = stepTimes ?? throw new ArgumentNullException(nameof(stepTimes));
        }

        public string Id { get; set; }
        public string Information { get; set; }
        public string Frame { get; set; }
        public DateTime CaptureTime { get; set; }
        public DateTime AlertTime { get; set; }
        public double MsExecutionTime { get; set; }
        public string Type { get; set; }
        public float Accuracy { get; set; }
        public SourceModel Source { get; set; }
        public string StepTimes { get; set; }

        public string toString()
        {
            return "Alert: " + Id + "\n SourceId: " + Source.Name + "\n Type: " + Type + "\n Information: " + Information + "\n Image: " + Frame + "\n Accuracy: " + Accuracy + "\n Initial time: " + CaptureTime;
        }
    }
}