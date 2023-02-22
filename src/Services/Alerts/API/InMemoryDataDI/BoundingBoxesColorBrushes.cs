using SixLabors.ImageSharp.Drawing.Processing;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.API.Injection
{
    public class BoundingBoxesColorBrushes
    {
        public BoundingBoxesColorBrushes()
        {
            Colors = new Dictionary<string, SolidBrush>();
        }

        public Dictionary<string, SolidBrush> Colors { get; set; }

        public SolidBrush GetColorByClass(string objectClass)
            => Colors.TryGetValue(objectClass, out var color) ? color :
               Colors.TryGetValue("default", out var defaultColor) ? defaultColor :
               Brushes.Solid(SixLabors.ImageSharp.Color.Black);
    }
}
