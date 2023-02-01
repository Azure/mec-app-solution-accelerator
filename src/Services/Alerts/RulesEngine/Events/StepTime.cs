namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events
{
    public class StepTime
    {
        public string StepName { get; set; }
        public DateTime StepStart { get; set; }
        public DateTime StepStop { get; set; }
        public long StepDuration { get; set; }
    }
}
