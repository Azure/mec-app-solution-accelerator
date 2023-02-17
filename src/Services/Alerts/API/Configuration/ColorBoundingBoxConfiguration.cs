using System.Security.Claims;

namespace Alerts.API.Configuration
{
    public class ColorBoundingBoxConfiguration
    {
        public List<ColorBoundingBox> Colors { get; set; }


        public class ColorBoundingBox
        {
            public string Class { get; set; }
            public string ColorName { get; set; }
            public SixLabors.ImageSharp.Color Color { get; set; }
        }

        public SixLabors.ImageSharp.Color GetColorByClass(string objectClass)
        {
            var colorInfo = Colors.FirstOrDefault(color => color.Class == objectClass);
            return colorInfo?.Color ?? Colors.FirstOrDefault(color => color.Class == "default")?.Color ?? SixLabors.ImageSharp.Color.Black;
        }
    }
}
