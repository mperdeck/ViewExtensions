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
        public static MvcHtmlString ViewLink(
            this HtmlHelper htmlHelper, string viewKey, string title = null, string cssClass = null)
        {
            string viewLink = Views.ByKey(viewKey).ViewLink(title, cssClass);
            return new MvcHtmlString(viewLink);
        }

        public static MvcHtmlString ViewMenu(this HtmlHelper htmlHelper)
        {
            string viewMenu = Views.ViewMenu();
            return new MvcHtmlString(viewMenu);
        }

    }
}
