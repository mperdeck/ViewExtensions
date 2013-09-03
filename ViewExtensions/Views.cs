using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewExtensions
{
    public class Views
    {
        private static Dictionairy<string, IViewInfo> _viewInfosByKey = new Dictionairy<string, IViewInfo>();
        private static Dictionairy<string, IViewInfo> _viewInfosByUrl = new Dictionairy<string, IViewInfo>();
        private static List<IViewInfo> _viewInfos = new List<IViewInfo>();

        public static void Load(string rootViewPath)
        {
            Load<ViewInfo>(rootViewPath);
        }

        public static void Load<T>(string rootViewPath) where T: IViewInfo
        {

        }

        public static IViewInfo ByKey(string pageKey)
        {
            return ByKey<ViewInfo>(pageKey);
        }

        public static T ByKey<T>(string pageKey) where T : class, IViewInfo
        {
            return null;
        }

        public static IViewInfo ByUrl(string url)
        {
            return ByUrl<ViewInfo>(url);
        }

        public static T ByUrl<T>(string url) where T : class, IViewInfo
        {
            return null;
        }

        public static string MenuHtml()
        {
            return null;
        }
    }
}
