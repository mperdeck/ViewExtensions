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
        /// <returns></returns>
        string ViewLink();
    }
}
