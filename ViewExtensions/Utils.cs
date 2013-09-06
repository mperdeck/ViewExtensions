using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewExtensions
{
    public static class Utils
    {
        public static string LinkHtml(string url, string title, string cssClass = null)
        {
            string linkHtml = 
                string.Format(@"<a {2} href=""{0}"">{1}</a>", 
                    url, HttpUtility.HtmlEncode(title), 
                    (string.IsNullOrEmpty(cssClass) ? "" : string.Format(@"class=""{0}""", cssClass)));
            
            return linkHtml;
        }

        public static string TrimTrailingIndex(string url)
        {
            const string indexPart = "index";

            if (url.ToLower().EndsWith(indexPart))
            {
                string trimmedUrl = url.Substring(0, url.Length - indexPart.Length);

                if (trimmedUrl != "/")
                {
                    trimmedUrl = trimmedUrl.TrimEnd(new char[] { '/' });
                }

                return trimmedUrl;
            }

            return url;
        }
    }
}
