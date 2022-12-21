namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base
{
    public class BaseEvent
    {
        public string Id { get; set; }
        public string EventName { get; set; }
        public string SourceId { get; set; }
        public long EveryTime { get; set; }
        public string Information { get; set; }
    }
}
