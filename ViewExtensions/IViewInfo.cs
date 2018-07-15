using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewExtensions
{
    /// <summary>
    /// Describes a single page.
    /// </summary>
    public interface IViewInfo
    {
        /// <summary>
        /// Key uniquely identifying this page.
        /// </summary>
        string Key { get; }

        /// <summary>
        /// The absolute url of the page. Does not contain the scheme or domain. 
        /// For example: /abc/def
        /// </summary>
        string Url { get; }

        /// <summary>
        /// The absolute path on the file system of the .cshtml file for this page.
        /// This is relative to the root of the site. For example,
        /// /Views/Contact.cshtml
        /// </summary>
        string ViewPath { get; }

        /// <summary>
        /// Title of the page.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Description of the page.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// This is a regex.
        /// If this is not null or empty, then the name of the current version is matched against this regex.
        /// If there is no match, the page is not shown in menus.
        /// </summary>
        string VersionNameRegex { get; }

        /// <summary>
        /// Pages with lower order get sorted before those with higher order.
        /// Pages with same order are sorted by their url.
        /// When Order is not given on the page, default is 1000.
        /// 
        /// Note that order is only relevant amongst siblings. A child will never be ordered
        /// away from its parent.
        /// </summary>
        int Order { get; }

        /// <summary>
        /// Child pages of this page. If the page has no children, this is the empty list.
        /// </summary>
        List<IViewInfo> Children { get; }

        /// <summary>
        /// Sets the properties of this IPageInfo based on the input parameters.
        /// </summary>
        /// <param name="viewFullPath">
        /// Absolute path on the file system of the .cshtml file.
        /// </param>
        /// <param name="viewFilesRootFullPath">
        /// Absolute path on the file system of the Views directory.
        /// </param>
        /// <param name="viewContent">
        /// Content of the .cshtml file.
        /// </param>
        void Load(string viewFullPath, string viewFilesRootFullPath, string viewContent);

        /// <summary>
        /// Generates html with a link with the url and title of this page.
        /// </summary>
        ///// <param name="htmlAttributes">
        ///// Additional html attributes to add to the anchor tag. Works the same way
        ///// as the htmlAttributes parameter of MVC's LinkExtensions.ActionLink.
        ///// </param>
        /// <param name="title">
        /// Title of the link. If null, title of the page itself is used. 
        /// </param>
        /// <param name="cssClass">
        /// CSS class to add to the anchor tag. If null, no class attribute is generated.
        /// </param>
        /// <param name="fragment">
        /// If not null, this is added to the url after a #.
        /// </param>
        /// <param name="onClick">
        /// If not null, an onclick handler is added to the link with this content.
        /// </param>
        /// <returns></returns>
        string ViewLink(string title = null, string cssClass = null, string fragment = null, string onClick = null);

        /// <summary>
        /// Returns the url of the view.
        /// </summary>
        /// <param name="fragment">
        /// If not null, this is added to the url after a #.
        /// </param>
        /// <returns></returns>
        string ViewUrl(string fragment = null);

        /// <summary>
        /// Returns true if this page should be shown in menus, given the versions that the
        /// site has been switched to by the user.
        /// If site versioning is not being used, this always returns true.
        /// </summary>
        /// <returns></returns>
        bool ShowInMenuForCurrentVersion();
    }
}
