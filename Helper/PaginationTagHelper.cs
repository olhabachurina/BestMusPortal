using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text;

namespace BestMusPortal.Helper
{
    [HtmlTargetElement("pagination")]
    public class PaginationTagHelper : TagHelper
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string Action { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var result = new StringBuilder();
            for (int i = 1; i <= TotalPages; i++)
            {
                result.Append($"<a href='/{Action}?page={i}' class='btn {(i == CurrentPage ? "btn-primary" : "btn-default")}'>{i}</a>");
            }
            output.Content.SetHtmlContent(result.ToString());
        }
    }
}