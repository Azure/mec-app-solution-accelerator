namespace Microsoft.MecSolutionAccelerator.Services.Alerts.Configuration
{
    public class ColorBoundingBoxConfiguration
    {
        public IEnumerable<ColorBoundingBox> Colors { get; set; }


        public class ColorBoundingBox
        {
            public string Class { get; set; }
            public string ColorName{ get; set; }
        }
    }
}
