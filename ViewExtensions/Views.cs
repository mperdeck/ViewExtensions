using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;

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
            if (!rootViewPath.StartsWith(Constants.ViewFilesRoot))
            {
                throw new ViewExtensionsException(
                    string.Format("rootViewPath ({0}) does not start with {1}", 
                    rootViewPath, Constants.ViewFilesRoot));
            }

            string viewFilesRootFullPath = HttpContext.Current.Server.MapPath(Constants.ViewFilesRoot);
            rootViewFullPath = HttpContext.Current.Server.MapPath(rootViewPath);

            _viewInfosByKey = new Dictionary<string, IViewInfo>();
            _viewInfosByUrl = new Dictionary<string, IViewInfo>();
            _viewInfos = new List<IViewInfo>();

            var viewFilePaths = Directory.EnumerateFiles(rootViewFullPath, "*.cshtml", SearchOption.AllDirectories);

            foreach (string viewFilePath in viewFilePaths)
            {
                // Exclude all files starting with _
                // because they are partials or _ViewStart.cshtml

                string fileName = Path.GetFileName(viewFilePath);
                if (fileName.StartsWith("_")) { continue; }

                string viewFileContent = File.ReadAllText(viewFilePath);

                T newViewInfo = new T();
                newViewInfo.Load(viewFilePath, viewFilesRootFullPath, viewFileContent);

                _viewInfos.Add(newViewInfo);
                _viewInfosByKey[newViewInfo.Key] = newViewInfo;
                _viewInfosByUrl[newViewInfo.Url] = newViewInfo;
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

            cleanedUrl = Utils.TrimTrailingIndex(cleanedUrl);

            if (!_viewInfosByUrl.ContainsKey(cleanedUrl))
            {
                throw new ViewExtensionsException(string.Format("Unknown url: {0}", url));
            }

            IViewInfo viewInfo = _viewInfosByUrl[cleanedUrl];
            return (T)viewInfo;
        }

        public static string ViewMenu()
        {
            var sortedViewInfos = _viewInfos.OrderBy(v => v.Url).AsEnumerable<IViewInfo>().ToList();
            int lengthRootViewFullPath = rootViewFullPath.Length;

            var sb = new StringBuilder();

            foreach (IViewInfo viewInfo in sortedViewInfos)
            {
                string cssClass = string.Format("level{0}", NbrForwardSlashes(viewInfo.Url));

                sb.AppendLine(viewInfo.ViewLink(null, cssClass));
            }

            return sb.ToString();
        }

        public static int NbrForwardSlashes(string url)
        {
            int nbrSlashes = url.Split(new char[] { '/' }).Length;
            return nbrSlashes;
        }
    }
}
