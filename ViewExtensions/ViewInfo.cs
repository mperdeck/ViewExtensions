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

            Url = Utils.TrimTrailingIndex(viewPathRelativeToViewRoot
                .Substring(0, viewPathRelativeToViewRoot.Length - Constants.ViewFileExtension.Length));

            // ---------------

            Title = ViewBagPageItem(@"Title", viewContent) ?? "";

            // If no key can be found on the page, use url instead.
            Key = ViewBagPageItem(@"Key", viewContent) ?? Url;

            Description = ViewBagPageItem(@"Description", viewContent) ?? "";
        }

        public string ViewLink(string title = null, string cssClass = null)
        {
            string currentUrl = Utils.TrimTrailingIndex(HttpContext.Current.Request.Url.AbsolutePath);
            string finalCssClass = cssClass;

            if (currentUrl == Url)
            {
                finalCssClass = (string.IsNullOrEmpty(cssClass) ? "" : (cssClass + " ")) + "selected";
            }

            return Utils.LinkHtml(Url, title ?? Title, finalCssClass);
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
