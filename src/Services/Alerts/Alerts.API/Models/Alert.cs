using Microsoft.MecSolutionAccelerator.Services.Commons;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Models
{
    [Table("Alert")]
    public class Alert : AEntity
    {
        public string Information { get; set; }
        public int Accuracy { get; set; }
        public string UrlVideoEncoded { get; set; }
        public DateTime AlertTriggerTimeIni { get; set; }
        public double lat { get; set; }
        public double @long { get; set; }
        public DateTime AlertTriggerTimeFin { get; set; }
    }
}
