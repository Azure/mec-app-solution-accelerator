namespace Microsoft.MecSolutionAccelerator.AlertsUI.Models
{
    public class AlertReducedModel
    {
        public AlertReducedModel(string id, DateTime captureTime, DateTime alertTime, double msExecutionTime, SourceModel source, string type, string information)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            CaptureTime = captureTime;
            AlertTime = alertTime;
            MsExecutionTime = msExecutionTime;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Information = information ?? throw new ArgumentNullException(nameof(information));
        }

        public string Id { get; set; }
        public DateTime CaptureTime { get; set; }
        public DateTime AlertTime { get; set; }
        public double MsExecutionTime { get; set; }
        public SourceModel Source { get; set; }
        public string Type { get; set; }
        public string Information { get; set; }                
    }
}
