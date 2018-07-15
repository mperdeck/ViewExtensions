using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web;

namespace ViewExtensions
{
    public class ViewInfo: IViewInfo
    {
        public string Key { get; protected set; }

        public string Url { get; protected set; }

        public string ViewPath { get; protected set; }

        public string Title { get; protected set; }

        public string Description { get; protected set; }

        public string VersionNameRegex { get; protected set; }

        public int Order { get; protected set; }

        public List<IViewInfo> Children { get; protected set; }

        public ViewInfo()
        {
            Children = new List<IViewInfo>();
        }

        public ViewInfo(string url): this()
        {
            Url = url;
        }

        public void Load(string viewFullPath, string viewFilesRootFullPath, string viewContent)
        {
            string viewPathRelativeToViewRoot = viewFullPath.Substring(viewFilesRootFullPath.Length)
                .Replace(@"\", "/");

            ViewPath = Constants.ViewFilesRoot + viewPathRelativeToViewRoot;

            // ---------------

            if (!ViewPath.EndsWith(Constants.ViewFileExtension))
            {
                throw new ViewExtensionsException(
                    string.Format("{0} does not end with {1}", viewFullPath, Constants.ViewFileExtension));
            }

            Url = UrlHelpers.TrimTrailingIndex(viewPathRelativeToViewRoot
                .Substring(0, viewPathRelativeToViewRoot.Length - Constants.ViewFileExtension.Length));

            // ---------------

            Title = ViewBagPageItem(@"Title", viewContent) ?? "";

            // If no key can be found on the page, use url instead.
            Key = ViewBagPageItem(@"Key", viewContent) ?? Url;

            Description = ViewBagPageItem(@"Description", viewContent) ?? "";

            VersionNameRegex = ViewBagPageItem(@"VersionNameRegex", viewContent) ?? "";

            string orderString = ViewBagPageItem(@"Order", viewContent) ?? "1000";
            Order = int.Parse(orderString);
        }

        public string ViewLink(string title = null, string cssClass = null, string fragment = null, string onClick = null)
        {
            string finalCssClass = cssClass;

            string finalUrl = Url;
            if (fragment != null)
            {
                finalUrl += "#" + fragment;
            }

            return HtmlHelpers.LinkHtml(finalUrl, title ?? Title, finalCssClass, onClick);
        }

        public string ViewUrl(string fragment = null)
        {
            string finalUrl = Url;
            if (fragment != null)
            {
                finalUrl += "#" + fragment;
            }

            return finalUrl;
        }

        public bool ShowInMenuForCurrentVersion()
        {
            string currentVersionName = PageVersions.CurrentVersion();
            if (currentVersionName == null) { return true; }

            if (string.IsNullOrEmpty(VersionNameRegex)) { return true; }

            if (Regex.IsMatch(currentVersionName, VersionNameRegex))
            {
                return true;
            }

            return false;
        }

        // Make sure that the item does not contain escaped characters or "",
        // because this method doesn't unescape anything.
        protected string ViewBagPageItem(string itemName, string viewContent)
        {
            string itemValue = PageItem(string.Format(@"ViewBag\.{0}\s*=\s*@?""(.+?)"";", itemName), viewContent);
            return itemValue;
        }

        protected string PageItem(string regex, string viewContent)
        {
            Match match = Regex.Match(viewContent, regex, RegexOptions.Singleline);
            if (!match.Success) { return null; }

            string item = match.Groups[1].Value;

            return item;
        }
    }
}
