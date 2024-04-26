namespace RulesEngine.Configuration
{
    public class RulesConfig
    {
        public string RuleName { get; set; }
        public int MinimumNumberOfFrames { get; set; }
        public float? MinimumThreshold { get; set; }
        public string DetectedObject { get; set; }
        public int? NumberfObjects { get; set; }
        public List<string> MultipleObjects { get; set; }
        public string Operator { get; set; }
    }
}
