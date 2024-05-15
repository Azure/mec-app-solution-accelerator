using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Alerts.UI.TagHelpers
{
    [HtmlTargetElement("icon-events", TagStructure = TagStructure.WithoutEndTag)]
    public class IconEventsTagHelper : TagHelper
    {
        public string Size { get; set; } = "24";
        public string StrokeWidth { get; set; } = "1";
        public string Color { get; set; } = "white";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "svg";
            output.Attributes.SetAttribute("xmlns", "http://www.w3.org/2000/svg");
            output.Attributes.SetAttribute("fill", "none");
            output.Attributes.SetAttribute("viewBox", "0 0 24 24");
            output.Attributes.SetAttribute("width", Size);
            output.Attributes.SetAttribute("height", Size);
            output.Attributes.SetAttribute("stroke-width", StrokeWidth);
            output.Attributes.SetAttribute("stroke", Color);

            var svgContent = @"<path stroke-linecap='round' stroke-linejoin='round'
            d='M3.75 3v11.25A2.25 2.25 0 006 16.5h2.25M3.75 3h-1.5m1.5 0h16.5m0 0h1.5m-1.5 0v11.25A2.25 2.25 0 0118 16.5h-2.25m-7.5 0h7.5m-7.5 0l-1 3m8.5-3l1 3m0 0l.5 1.5m-.5-1.5h-9.5m0 0l-.5 1.5M9 11.25v1.5M12 9v3.75m3-6v6'
            />";
            output.Content.SetHtmlContent(svgContent);
            output.TagMode = TagMode.StartTagAndEndTag;
        }
    }
}