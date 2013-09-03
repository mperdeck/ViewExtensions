using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;

namespace ViewExtensions
{
    public static class LinkExtensions
    {
        public static MvcHtmlString ViewLink(this HtmlHelper htmlHelper, string viewKey)
        {
            return ViewLink(htmlHelper, viewKey, null);
        }

        public static MvcHtmlString ViewLink(this HtmlHelper htmlHelper, string viewKey, IDictionary<string, Object> htmlAttributes)
        {
            return new MvcHtmlString("aaa");
        }

        public static MvcHtmlString ViewMenu(this HtmlHelper htmlHelper)
        {
            return null;
        }

    }
}
