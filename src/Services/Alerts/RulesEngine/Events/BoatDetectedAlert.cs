using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events
{
    public class BoatDetectedAlert : BaseEvent
    {
        public string Name => "boat";
        public List<DetectionFrame> Frames { get; set; }
        public DateTime AlertTriggerTimeIni { get; set; } //First detection frame date.
        public DateTime AlertTriggerTimeFin { get; set; } //Last detection frame date.
    }
}
