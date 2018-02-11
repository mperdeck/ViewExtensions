using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewExtensions;

namespace ViewExtensions.Tests
{
    public class Helpers
    {
        public static string SiteRootPath = @"X:\Dev";

        public static string MapPath(string url)
        {
            return SiteRootPath + @"\" + url.Replace("/", "\\").Trim(new char[] { '\\' });
        }

        public static IEnumerable<string> AllMockedCSHtmlFilesInDirectory(IEnumerable<MockFile> mockFiles)
        {
            return mockFiles.Select(mf => mf.Path);
        }

        public static string ReadAllMockText(IEnumerable<MockFile> mockFiles, string path)
        {
            return mockFiles.Where(mf=>mf.Path == path).Select(mf => mf.Content).First();
        }

        public static void MockLoad(IEnumerable<MockFile> mockFiles, string currentUrl)
        {
            Views.Load<ViewInfo>(
                "",
                MapPath,
                path => AllMockedCSHtmlFilesInDirectory(mockFiles),
                path => ReadAllMockText(mockFiles, path),
                ()=> currentUrl);
        }
    }
}
