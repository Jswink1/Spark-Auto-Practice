using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SparkAutoPractice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SparkAutoPractice.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        // UrlHelper builds the URL
        private IUrlHelperFactory urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }

        // ViewContext provides access to HTTP context, request, and response
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public Pagination PageModel { get; set; }
        public string PageAction { get; set; } // Defines what action is invoked when a user selects another page
        public string PageClass { get; set; } // Initial CSS properties
        public string PageClassNormal { get; set; } // CSS properties for pages that are not selected
        public string PageClassSelected { get; set; } // CSS properties for selected page

        // TagHelperContext contains the information related to execution of a TagHelper.
        // TagHelperOutput represents the output of a TagHelper.
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");

            for (int i = 1; i <= PageModel.TotalPages; i++)
            {
                // Create anchor tag with a url.
                // When a url is created, a ":" is appended to the end, and must be replaced with the count number
                TagBuilder tag = new TagBuilder("a");
                string url = PageModel.UrlParam.Replace(":", i.ToString());
                tag.Attributes["href"] = url;

                // Apply Css
                tag.AddCssClass(PageClass);
                tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);

                // The text to display is the count number
                tag.InnerHtml.Append(i.ToString());

                // Append the Tag
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}
