namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Models
{
    public class StepTimeAsDate
    {
        public string StepName { get; set; }
        public DateTime StepStart { get; set; }
        public DateTime StepStop { get; set; }
        public long StepDuration { get; set; }
    }
}
