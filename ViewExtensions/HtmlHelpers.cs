using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewExtensions
{
    public static class HtmlHelpers
    {
        public static string LinkHtml(string url, string title, string cssClass = null, string onClick = null)
        {
            string onClickHtml = "";
            if (!string.IsNullOrEmpty(onClick))
            {
                onClickHtml = " onclick='" + HttpUtility.JavaScriptStringEncode(onClick) + "'";
            }

            string linkHtml = 
                string.Format(@"<a {2} href=""{0}""{3}>{1}</a>", 
                    url, HttpUtility.HtmlEncode(title), 
                    ClassAttribute(cssClass),
                    onClickHtml);
            
            return linkHtml;
        }

        public static string TableRowLinkedHtml(string url, string title, string description)
        {
            string rowHtml =
                string.Format(@"<tr><td valign=""top"">{0}</td><td>{1}</td></tr>",
                    LinkHtml(url, title), HttpUtility.HtmlEncode(description));

            return rowHtml;
        }

        public static string TableRowHtml(string title, string subTitle, params string[] rowValues)
        {
            var rowHtml = new StringBuilder();

            rowHtml.AppendLine(
                string.Format(@"<tr><td valign=""top"">{0}{1}</td>",
                    HttpUtility.HtmlEncode(title),
                    string.IsNullOrEmpty(subTitle) ? "" : string.Format("<br /><small>{0}</small>", subTitle)));

            foreach(string rowValue in rowValues)
            {
                if (rowValue != null)
                {
                    // Do not html encode row values. That way, you can use links in the row values.
                    rowHtml.AppendLine(string.Format(@"<td valign=""top"">{0}</td>", rowValue));
                }
            }

            rowHtml.AppendLine("</tr>");

            return rowHtml.ToString();
        }

        public static string ClassAttribute(string cssClass)
        {
            return string.IsNullOrEmpty(cssClass) ? "" : string.Format(@"class=""{0}""", cssClass);
        }
    }
}
