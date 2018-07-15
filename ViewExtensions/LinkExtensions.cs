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
        public static string ViewLink(
            string viewKey, string title = null,
            string cssClass = null, string fragment = null, string onClick = null)
        {
            string viewLink = Views.ByKey(viewKey).ViewLink(title, cssClass, fragment, onClick);
            return viewLink;
        }

        public static MvcHtmlString ViewLink(
            this HtmlHelper htmlHelper, string viewKey, string title = null, 
            string cssClass = null, string fragment = null, string onClick = null)
        {
            return new MvcHtmlString(ViewLink(viewKey, title, cssClass, fragment, onClick));
        }

        public static string ViewUrl(this HtmlHelper htmlHelper, string viewKey, string fragment = null)
        {
            string viewUrl = Views.ByKey(viewKey).ViewUrl(fragment);
            return viewUrl;
        }

        public static MvcHtmlString ViewMenu(this HtmlHelper htmlHelper)
        {
            string viewMenu = Views.ViewMenu();
            return new MvcHtmlString(viewMenu);
        }

        public static MvcHtmlString Breadcrumbs(this HtmlHelper htmlHelper)
        {
            string breadcrumbs = Views.Breadcrumbs();
            return new MvcHtmlString(breadcrumbs);
        }

        public static MvcHtmlString ChildrenTable(
            this HtmlHelper htmlHelper, 
            string column1Header = "Member", 
            string cssClass = null)
        {
            string tableHtml = Views.TableChildrenCurrentPage(column1Header, cssClass);
            return new MvcHtmlString(tableHtml);
        }

        public static MvcHtmlString TableRowHtml(
            this HtmlHelper htmlHelper,
            string title, string subTitle, params string[] rowValues)
        {
            string tableRowHtml = HtmlHelpers.TableRowHtml(title, subTitle, rowValues);
            return new MvcHtmlString(tableRowHtml);
        }
    }
}
