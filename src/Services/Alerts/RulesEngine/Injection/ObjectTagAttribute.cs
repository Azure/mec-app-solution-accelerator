namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Injection
{
    public class ObjectTagAttribute : Attribute
    {
        public ObjectTagAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; set; }
    }
}
