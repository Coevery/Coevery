using System;
using System.Reflection;
using System.Web;
using Orchard.ContentManagement;

namespace Orchard.CulturePicker.Services {
    public static class Utils {
        //returns url based on the current http request
        //takes into account, that url can contain application path

        public static string GetReturnUrl(HttpRequestBase request) {
            if (request.UrlReferrer == null) {
                return String.Empty;
            }

            string localUrl = GetAppRelativePath(request.UrlReferrer.AbsolutePath, request);
            //support for pre-encoded Unicode urls
            return HttpUtility.UrlDecode(localUrl);
        }

        //Translates an ASP.NET path like /myapp/subdir/page.aspx 
        //into an application relative path: subdir/page.aspx. The
        //path returned is based of the application base and 

        public static string GetAppRelativePath(string logicalPath, HttpRequestBase request) {
            if (request.ApplicationPath == null) {
                return String.Empty;
            }

            logicalPath = logicalPath.ToLower();
            string appPath = request.ApplicationPath.ToLower();
            if (appPath != "/") {
                appPath += "/";
            }
            else {
                // Root web relative path is empty
                return logicalPath.Substring(1);
            }

            return logicalPath.Replace(appPath, "");
        }

        public static Version GetOrchardVersion() {
            return new AssemblyName(typeof (ContentItem).Assembly.FullName).Version;
        }
    }
}