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

        /// <summary>
        /// Takes a url, removes everything that comes after the domain and returns the result.
        /// So
        /// http://domain.com/xxx/zzz
        /// is turned into
        /// http://domain.com
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string DomainOnlyUrl(string url)
        {
            int locationDoubleSlash = url.IndexOf("//");
            int startSearchEndDomain = (locationDoubleSlash == -1) ? 0 : (locationDoubleSlash + 2);

            int locationEndDomain = url.IndexOf("/", startSearchEndDomain);
            if (locationEndDomain == -1)
            {
                // No slash found after domain, so url only contains domain
                return url;
            }

            string domainOnlyUrl = url.Substring(0, locationEndDomain);
            return domainOnlyUrl;
        }
    }
}
