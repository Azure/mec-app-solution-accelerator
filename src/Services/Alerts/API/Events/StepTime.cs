namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Events
{
    public class StepTime
    {
        public string StepName { get; set; }
        public DateTime StepStart { get; set; }
        public DateTime StepStop { get; set; }
        public double StepDuration { get; set; }
    }
}
