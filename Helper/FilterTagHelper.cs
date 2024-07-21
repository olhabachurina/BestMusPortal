using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace BestMusPortal.Helper
{
    [HtmlTargetElement("filter")]
    public class FilterTagHelper : TagHelper
    {
        public string Action { get; set; }
        public string FilterBy { get; set; }
        public IEnumerable<string> Options { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var result = new StringBuilder();
            result.Append($"<form method='get' action='/{Action}'>");
            result.Append("<select name='filter'>");
            foreach (var option in Options)
            {
                result.Append($"<option value='{option}'>{option}</option>");
            }
            result.Append("</select>");
            result.Append("<button type='submit'>Filter</button>");
            result.Append("</form>");
            output.Content.SetHtmlContent(result.ToString());
        }
    }
}