using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Models
{
    [Table("Detection")]
    public class Detection : AEntity
    {
        public string EventType { get; set; }
        public long EveryTime { get; set; }
        public float Confidence { get; set; }
    }
}
