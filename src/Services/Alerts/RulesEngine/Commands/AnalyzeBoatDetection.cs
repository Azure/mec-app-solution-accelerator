using MediatR;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;
using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Injection;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Commands
{
    [ObjectTag("boat")]
    public class AnalyzeBoatDetection : IRequest
    {
        public string Id { get; set; }
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
