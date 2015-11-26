using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web;
using System.Text;

// ----------------------------------------------------
// To test sub domains, create domains in 
// C:\WINDOWS\system32\drivers\etc\hosts
// ----------------------------------------------------

namespace ViewExtensions
{
    public static class PageVersions
    {
        private static IEnumerable<VersionInfo> _versionInfos = null;
        private static bool _useCookies = false;
        private static bool _useSubDomain = false;

        public class VersionInfo
        {
            public string VersionUrlName { get; set; } // Used in url

            // Overrides VersionUrlName. If this is not null, the link in a version switcher always has this url.
            // If you're on a page with this url and use the version switcher, this version will be shown as the current version.
            public string VersionUrlOverride { get; set; } 
            
            public string VersionName { get; set; } // Used in C# code
            public string Caption { get; set; } // Used in version switcher
            public bool IsDefault { get; set; }
        }

        private const string VersionUrlParam = "version";
        private const string CookieName = "version";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="versionInfos"></param>
        /// <param name="useCookies">
        /// If true, the current version choice is stored in a cookie.
        /// When no version choice can be determined, the contents of the cookie is used.
        /// 
        /// If false, no cookies are used.
        /// </param>
        /// <param name="useSubDomain">
        /// If true, loading a new version means loading a sub domain.
        /// And the current version is determined from the currrent sub domain (if there is one).
        /// 
        /// If false, a query string param is used.
        /// </param>
        public static void Load(IEnumerable<VersionInfo> versionInfos, bool useCookies, bool useSubDomain)
        {
            _versionInfos = versionInfos;
            _useCookies = useCookies;
            _useSubDomain = useSubDomain;
        }

        /// <summary>
        /// Returns the name of the currently selected version.
        /// </summary>
        /// <returns></returns>
        public static string CurrentVersion()
        {
            if (_versionInfos == null)
            {
                return null;
            }

            // First try get version from url

            Uri url = HttpContext.Current.Request.Url;
            VersionInfo versionInfo = GetCurrentVersionInfoFromUrl(url);

            if (versionInfo != null)
            {
                string versionName = versionInfo.VersionName;

                if (_useCookies)
                {
                    // Set cookie, so when other pages are opened user gets same version
                    HttpContext.Current.Response.Cookies[CookieName].Value = versionName;
                    HttpContext.Current.Request.Cookies[CookieName].Expires = DateTime.Now.AddYears(1);
                }

                return versionName;
            }

            if (_useCookies)
            {
                // Then try cookie

                string versionName = HttpContext.Current.Response.Cookies[CookieName].Value; 
                if (!String.IsNullOrEmpty(versionName))
                {
                    if (_versionInfos.Any(v => v.VersionName == versionName))
                    {
                        return versionName;
                    }
                }
            }

            // If no cookie, use default

            string defaultVersionName = _versionInfos.Single(v => v.IsDefault).VersionName;
            return defaultVersionName;
        }

        public static MvcHtmlString VersionSwitcher(this HtmlHelper htmlHelper)
        {
            if (_versionInfos == null)
            {
                return null;
            }

            var versionName = CurrentVersion();
            var sb = new StringBuilder();

            foreach (var versionInfo in _versionInfos)
            {
                if (versionInfo.VersionName == versionName)
                {
                    // Class is used for bootstrap styling
                    sb.AppendFormat(@"<span class=""btn btn-primary"">{0}</span>", versionInfo.Caption);
                }
                else if (versionInfo.VersionUrlOverride != null)
                {
                    // Class is used for bootstrap styling
                    sb.AppendFormat(
                        @"<a class=""btn btn-default"" href=""{0}"">{1}</a>", versionInfo.VersionUrlOverride, versionInfo.Caption);
                }
                else
                {
                    Uri url = HttpContext.Current.Request.Url;

                    // Class is used for bootstrap styling
                    sb.AppendFormat(
                        @"<a class=""btn btn-default"" href=""{0}"">{1}</a>", UrlHelpers.DomainOnlyUrl(UrlWithVersionUrlName(url, versionInfo.VersionUrlName)), versionInfo.Caption);
                }
            }

            return new MvcHtmlString(sb.ToString());
        }

        private static VersionInfo GetCurrentVersionInfoFromUrl(Uri url)
        {
            string versionUrlName = null;

            // Check versions that have a url override

            string currentUrl = url.ToString();
            string currentUrlWithoutWww = currentUrl.Replace("http://www.", "http://");
            VersionInfo versionInfo = _versionInfos.SingleOrDefault(v => (v.VersionUrlOverride == currentUrl));

            if (versionInfo != null)
            {
                return versionInfo;
            }

            // No direct match with url override found. Try to use version url name.

            if (_useSubDomain)
            {
                versionUrlName = RequestSubdomain(url);
            }
            else
            {
                versionUrlName = (string)HttpContext.Current.Request.QueryString[VersionUrlParam];
            }

            if (versionUrlName == null)
            {
                return null;
            }

            versionInfo = _versionInfos.SingleOrDefault(v => (v.VersionUrlName == versionUrlName));
            return versionInfo;
        }

        public static string UrlWithVersionUrlName(Uri url, string versionUrlName)
        {
            if (!_useSubDomain)
            {
                return string.Format("?{0}={1}", VersionUrlParam, versionUrlName);
            }

            // Take current uri, and replace sub domain with version url name.
            //
            // This assumes that the pattern "//subdomain." (2 x forward slash, followed by sub domain, followed by .)
            // doesn't appear anywhere else in the uri.

            // If new version is the default, do not use a sub domain
            string newSubdomainWithDot = versionUrlName + ".";
            if (_versionInfos.Single(v => v.VersionUrlName == versionUrlName).IsDefault)
            {
                newSubdomainWithDot = "";
            }

            string currentUri = url.ToString();
            string currentSubDomain = RequestSubdomain(url);
            string newUri = "";

            if (string.IsNullOrEmpty(currentSubDomain))
            {
                // there is currently no sub domain
                newUri = currentUri.Replace("//", "//" + newSubdomainWithDot);
            }
            else
            {
                // there is currently a sub domain
                newUri = currentUri.Replace("//" + currentSubDomain + ".", "//" + newSubdomainWithDot);
            }

            return newUri;
        }

        /// <summary>
        /// Returns the sub domain in the passed in URL.
        /// 
        /// Returns null if there is no sub domain.
        /// 
        /// Assumes that the domain is not country specific. 
        /// For example, jsnlog.com but not jsnlog.com.au.
        /// 
        /// Note that the domain could be localhost, in which case you can
        /// have js.localhost, etc.
        /// </summary>
        /// <returns></returns>
        private static string RequestSubdomain(Uri url)
        {
            string[] uriParts = url.Host.Split(new[] { '.' });

            // If there are only 2 or 1 parts in the host name, you can't have a sub domain.
            // However, if the last part is localhost, you can have 2 parts.

            int nbrParts = uriParts.Length;
            int minimumNbrParts = (uriParts[nbrParts - 1].ToLower() == "localhost") ? 2 : 3;

            if (uriParts.Length < minimumNbrParts)
            {
                return null;
            }

            // Assume that the first part is the sub domain. For example,
            // js.jsnlog.com

            return uriParts[0];
        }
    }
}

