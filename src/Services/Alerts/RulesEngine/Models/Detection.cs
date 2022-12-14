using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Models
{
    [Table("Detection")]
    public class Detection : AEntity
    {
        public string Information { get; set; }
        public string UrlVideoEncoded { get; set; }
        public string Frame { get; set; }
        public string EventType { get; set; }
        public string EventName { get; set; }
        public string SourceId { get; set; }
        public long EveryTime { get; set; }
        public string Type { get; set; }
        public List<BoundingBox> BoundingBoxes { get; set; }
    }
}
