namespace RulesEngine.InMemoryDataDI
{
    public class RuleTagAttribute : Attribute
    {
        public RuleTagAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; set; }
    }
}