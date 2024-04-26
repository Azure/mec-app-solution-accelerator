namespace RulesEngine.Events.Base
{
    public class BaseEvent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SourceId { get; set; }
        public long EveryTime { get; set; }
        public string Information { get; set; }
    }
}
