using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Goui.AspNetCore.TagHelpers
{
    public class GouiTagHelper : TagHelper
    {
        public Goui.Html.Element Element { get; set; }

        public override void Process (TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent (Element.OuterHtml);
        }
    }
}
