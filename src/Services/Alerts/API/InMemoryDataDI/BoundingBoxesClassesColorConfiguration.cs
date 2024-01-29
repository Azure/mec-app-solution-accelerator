using Microsoft.MecSolutionAccelerator.Services.Alerts.Configuration;
using SixLabors.ImageSharp.Drawing.Processing;

namespace Microsoft.MecSolutionAccelerator.Services.Alerts.API.Injection
{
    public static class BoundingBoxesClassesColorConfiguration
    {
        public static void AddBoundingBoxesColorConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var colorsConfig = configuration.GetSection("ColorBoundingBoxes").Get<ColorBoundingBoxConfiguration>();

            AddColorsConfigurationDictionary(services, colorsConfig);
        }

        private static void AddColorsConfigurationDictionary(IServiceCollection services, ColorBoundingBoxConfiguration configuration)
        {
            var colorsDictionary = configuration.Colors.ToDictionary(color => color.Class, color => Brushes.Solid(SixLabors.ImageSharp.Color.Parse(color.ColorName)));
            services.AddSingleton(new BoundingBoxesColorBrushes { Colors = colorsDictionary });
        }
    }
}
