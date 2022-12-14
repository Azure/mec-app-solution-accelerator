﻿using Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events.Base;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.RulesEngine.Events
{
    public class PlaneDetectedAlert : BaseEvent
    {
        public List<DetectionFrame> Frames { get; set; }
        public DateTime AlertTriggerTimeIni { get; set; } //First detection frame date.
        public DateTime AlertTriggerTimeFin { get; set; } //Last detection frame date.
    }
}