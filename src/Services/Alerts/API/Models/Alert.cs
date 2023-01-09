using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Models
{
    [Table("Alert")]
    public class Alert : AEntity
    {
        public string Information { get; set; }
        public string Frame { get; set; }
        public DateTime AlertTriggerTimeIni { get; set; }
        public string Type { get; set; }
        public float Accuracy { get; set; }
        public Source Source { get; set; }
    }
}
