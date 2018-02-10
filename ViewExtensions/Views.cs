﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace ViewExtensions
{
    public class Views
    {
        private static Dictionary<string, IViewInfo> _viewInfosByKey = null;
        private static Dictionary<string, IViewInfo> _viewInfosByUrl = null;
        private static List<IViewInfo> _viewInfos = null;

        private static string rootViewFullPath = null;

        /// <summary>
        /// Call this method when the site starts.
        /// Loads info about the views into an internal structure.
        /// </summary>
        /// <param name="rootViewPath">
        /// All view files in this directory and its subdirectories will be visited.
        /// 
        /// This directory must start with 
        /// ~/Views
        /// </param>
        public static void Load(string rootViewPath)
        {
            Load<ViewInfo>(rootViewPath);
        }

        public static void Load<T>(string rootViewPath) where T : IViewInfo, new()
        {
            Load<T>(
                rootViewPath,
                url => HttpContext.Current.Server.MapPath(url),
                dirPath => Directory.EnumerateFiles(dirPath, "*.cshtml", SearchOption.AllDirectories),
                path => File.ReadAllText(path));
        }

        public static void Load<T>(
            string rootViewPath,
            Func<string, string> mapPath,
            Func<string, IEnumerable<string>> allCSHtmlFilesInDirectory,
            Func<string, string> readAllText) where T : IViewInfo, new()
        {
            if (!rootViewPath.StartsWith(Constants.ViewFilesRoot))
            {
                throw new ViewExtensionsException(
                    string.Format("rootViewPath ({0}) does not start with {1}", 
                    rootViewPath, Constants.ViewFilesRoot));
            }

            string viewFilesRootFullPath = mapPath(Constants.ViewFilesRoot);
            rootViewFullPath = mapPath(rootViewPath);

            _viewInfosByKey = new Dictionary<string, IViewInfo>();
            _viewInfosByUrl = new Dictionary<string, IViewInfo>();
            _viewInfos = new List<IViewInfo>();

            // Order the file paths alphabetically, so parent pages will come before their children.
            var viewFilePaths =
                allCSHtmlFilesInDirectory(rootViewFullPath)
                .OrderBy(p=>p);

            foreach (string viewFilePath in viewFilePaths)
            {
                // Exclude all files starting with _
                // because they are partials or _ViewStart.cshtml

                string fileName = Path.GetFileName(viewFilePath);
                if (fileName.StartsWith("_")) { continue; }

                string viewFileContent = readAllText(viewFilePath);

                T newViewInfo = new T();
                newViewInfo.Load(viewFilePath, viewFilesRootFullPath, viewFileContent);

                _viewInfos.Add(newViewInfo);
                _viewInfosByKey[newViewInfo.Key] = newViewInfo;
                _viewInfosByUrl[newViewInfo.Url] = newViewInfo;

                string parentUrl = ParentUrl(newViewInfo.Url);
                if (_viewInfosByUrl.ContainsKey(parentUrl))
                {
                    _viewInfosByUrl[parentUrl].Children.Add(newViewInfo);
                }
            }
        }

        /// <summary>
        /// Retrieves the info for a page by its key.
        /// </summary>
        /// <param name="viewKey"></param>
        /// <returns></returns>
        public static IViewInfo ByKey(string viewKey)
        {
            return ByKey<ViewInfo>(viewKey);
        }

        public static T ByKey<T>(string viewKey) where T : class, IViewInfo
        {
            if (!_viewInfosByKey.ContainsKey(viewKey))
            {
                throw new ViewExtensionsException(string.Format("Unknown view key: {0}", viewKey));
            }

            IViewInfo viewInfo = _viewInfosByKey[viewKey];
            return (T)viewInfo;
        }

        /// <summary>
        /// Retrieves the info for a page by its url.
        /// </summary>
        /// <param name="pageKey"></param>
        /// <returns></returns>
        public static IViewInfo ByUrl(string url)
        {
            return ByUrl<ViewInfo>(url);
        }

        public static T ByUrl<T>(string url) where T : class, IViewInfo
        {
            string cleanedUrl = url ?? "";

            if (!cleanedUrl.StartsWith("/")) { cleanedUrl = "/" + cleanedUrl; }

            cleanedUrl = UrlHelpers.TrimTrailingIndex(cleanedUrl);

            if (!_viewInfosByUrl.ContainsKey(cleanedUrl))
            {
                throw new ViewExtensionsException(string.Format("Unknown url: {0}", url));
            }

            IViewInfo viewInfo = _viewInfosByUrl[cleanedUrl];
            return (T)viewInfo;
        }

        public static string ViewMenu()
        {
            var pagesToShowInMenu = _viewInfos.Where(v => v.ShowInMenuForCurrentVersion());
            var sortedViewInfos = pagesToShowInMenu.OrderBy(v => v.Url).AsEnumerable<IViewInfo>().ToList();
            int lengthRootViewFullPath = rootViewFullPath.Length;

            var sb = new StringBuilder();

            foreach (IViewInfo viewInfo in sortedViewInfos)
            {
                string cssClass = string.Format("level{0}", NbrComponents(viewInfo.Url) + 1);

                sb.AppendLine(viewInfo.ViewLink(null, cssClass));
            }

            return sb.ToString();
        }

        public static string Breadcrumbs()
        {
            string currentUrl = UrlHelpers.CurrentUrl();
            int nbrComponents = NbrComponents(currentUrl);

            // If there is only one component, there is no need to have a breadcrumb.
            // Note that a url starts with /Documentation, so if the user is in Configuration (top level section,
            // no breadcrumb needed), the url is /Documentation/Configuration
            if (nbrComponents <= 2)
            {
                return "";
            }

            // Will contain infos for each breadcrumb element
            var viewInfos = new List<IViewInfo>(nbrComponents);

            List<string> components = Components(currentUrl);

            // Skip the first component, because that is always Documentation,
            // and /Documentation doesn't match a page and is not a url stored in viewInfos.
            for(int i = 1; i < nbrComponents; i++)
            {
                List<string> componentsThisUrl = components.Take(i + 1).ToList();
                string url = "/" + string.Join("/", componentsThisUrl);
                IViewInfo viewInfo = ByUrl(url);
                viewInfos.Add(viewInfo);
            }

            // All but the last view info becomes an anchor. The last one is just the title.
            List<string> breadCrumbComponents =
                viewInfos.Take(viewInfos.Count - 1).Select(c=>c.ViewLink(null, null, null)).ToList();

            breadCrumbComponents.Add(
                "<span class=\"breadcrumb-final-title\">" + 
                HttpUtility.HtmlEncode(viewInfos.Last().Title) + 
                "</span>");

            string breakCrumbHtml = string.Join(" <span class=\"breadcrumb-separator\">&#9654;</span> ", breadCrumbComponents);

            return breakCrumbHtml;
        }

        /// <summary>
        /// Generates a table listing all children (but not their grand children, etc)
        /// of the current page.
        /// 
        /// Whether a page is a child depends on its url:
        /// /a/b     current page
        /// /a/b/c   child page
        /// 
        /// The table has 2 columns:
        /// 1) link to the child page
        /// 2) description of the child
        /// </summary>
        /// <param name="column1Header">
        /// Header of the first column. Header of the second column is always "Description".
        /// </param>
        /// <param name="cssClass">
        /// Class to be given to the table tag. Leave null if no class to be given.
        /// </param>
        /// <returns>
        /// Html of the table. Null if there are no children.
        /// </returns>
        public static string TableChildrenCurrentPage(string column1Header = "Member", string cssClass = null)
        {
            string currentUrl = UrlHelpers.CurrentUrl();
            int currentUrlNbrComponents = NbrComponents(currentUrl);
            int childUrlNbrComponents = currentUrlNbrComponents + 1;

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("<table {0}>", HtmlHelpers.ClassAttribute(cssClass)));
            sb.AppendLine(string.Format(@"<thead><tr><th align=""left"">{0}</th><th align=""left"">Description</th></tr></thead>", column1Header));
            sb.AppendLine("<tbody>");

            bool viewInfosFound = false;
            _viewInfos
                .Where(v => v.Url.StartsWith(currentUrl) && 
                            (NbrComponents(v.Url) == childUrlNbrComponents) &&
                            v.ShowInMenuForCurrentVersion())
                .OrderBy(v => v.Order)
                .ThenBy(v => v.Url)
                .ToList()
                .ForEach(v =>
                {
                    sb.AppendLine(HtmlHelpers.TableRowLinkedHtml(v.Url, v.Title, v.Description));
                    viewInfosFound = true;
                });

            if (!viewInfosFound) { return null; }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");

            return sb.ToString();
        }

        public static List<string> Components(string url)
        {
            List<string> components = url.Split(new char[] { '/' }).ToList();
            int nbrComponents = components.Count();

            // Url starts with /Documentation/, so first component is empty and second component doesn't get stored
            var componentsExceptFirst = components.Skip(1).Take(nbrComponents - 1).ToList();

            return componentsExceptFirst;
        }

        public static int NbrComponents(string url)
        {
            int nbrComponents = Components(url).Count();
            return nbrComponents;
        }

        /// <summary>
        /// Returns the parent url. For example, if passed in url is
        /// /a/b/c
        /// then parent url is
        /// /a/b
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ParentUrl(string url)
        {
            // Should never be -1
            int idxLastSlash = url.LastIndexOf('/');

            string parent = url.Substring(0, idxLastSlash);
            return parent;
        }
    }
}
