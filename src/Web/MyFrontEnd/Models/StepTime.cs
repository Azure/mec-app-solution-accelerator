using System;
using System.Xml.Linq;

namespace MyFrontEnd.Models
{
    public class StepTime
    {
        public string StepName { get; set; }
        public DateTime StepStart { get; set; }
        public DateTime StepStop { get; set; }
        public int StepDuration { get; set; }
    }
}

