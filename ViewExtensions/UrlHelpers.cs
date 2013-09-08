using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ViewExtensions
{
    public static class UrlHelpers
    {
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

        public static string CurrentUrl()
        {
            string currentUrl = TrimTrailingIndex(HttpContext.Current.Request.Url.AbsolutePath);
            return currentUrl;
        }
    }
}
