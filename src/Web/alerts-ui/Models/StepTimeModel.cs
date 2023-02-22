using System;
using System.Xml.Linq;

namespace Microsoft.MecSolutionAccelerator.AlertsUI.Models
{
    public class StepTimeModel
    {
        public string StepName { get; set; }
        public DateTime StepStart { get; set; }
        public DateTime StepStop { get; set; }
        public int StepDuration { get; set; }
    }
}

