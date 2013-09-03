using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewExtensions
{
    public class ViewInfo: IViewInfo
    {
        /// <summary>
        /// blah c
        /// </summary>
        public string Url
        {
            get { throw new NotImplementedException(); }
        }

        public string ViewPath
        {
            get { throw new NotImplementedException(); }
        }

        public string Title
        {
            get { throw new NotImplementedException(); }
        }

        public string Description
        {
            get { throw new NotImplementedException(); }
        }

        public void Load(string viewPath, string viewContent)
        {
            throw new NotImplementedException();
        }

        public string ViewLink(IDictionary<string, Object> htmlAttributes)
        {
            throw new NotImplementedException();
        }
    }
}
