namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Models
{
    public class AlertMinimized 
    {
        public Guid Id { get; set; }
        public string Information { get; set; }
        public DateTime CaptureTime { get; set; }
        public DateTime AlertTime { get; set; }
        public double MsExecutionTime { get; set; }
        public string Type { get; set; }
        public float Accuracy { get; set; }
    }
}
