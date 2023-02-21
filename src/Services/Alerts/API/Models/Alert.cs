using Microsoft.MecSolutionAccelerator.Services.Alerts.Events;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Models
{
    [Table("Alert")]
    public class Alert : AEntity
    {
        public string Information { get; set; }
        public string Frame { get; set; }
        public DateTime CaptureTime { get; set; }
        public DateTime AlertTime { get; set; }
        public double MsExecutionTime { get; set; }
        public string Type { get; set; }
        public float Accuracy { get; set; }
        public Source Source { get; set; }
        public string StepTimes { get; set; }
        public IEnumerable<StepTimeAsDate> StepTimes2 { get; set; }
        public IEnumerable<DetectionClass> MatchesClasses { get; set; }
    }
}
