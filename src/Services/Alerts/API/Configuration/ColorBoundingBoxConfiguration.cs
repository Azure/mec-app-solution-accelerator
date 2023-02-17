namespace Alerts.API.Configuration
{
    public class ColorBoundingBoxConfiguration
    {
        public ColorBoundingBox Colors { get; set; }


        public class ColorBoundingBox
        {
            public string Class { get; set; }
            public SixLabors.ImageSharp.Color Color { get; set; }
        }
    }
}
