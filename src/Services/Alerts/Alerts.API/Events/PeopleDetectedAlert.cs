using Microsoft.MecSolutionAccelerator.Services.Alerts.Events.Base;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Events
{
    public class PeopleDetectedAlert : BaseEvent
    {
        public List<DetectionFrame> Frames { get; set; }
        public DateTime AlertTriggerTimeIni { get; set; } //First detection frame date.
        public DateTime AlertTriggerTimeFin { get; set; } //Last detection frame date.
        public long NumberOfPeople { get; set; }
    }
}
