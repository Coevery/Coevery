using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.Mvc.Html;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Coevery.Core.Extensions {
    public static class CoeveryHtmlExtensions {
        public static MvcForm BeginFormAntiForgeryPost(this HtmlHelper htmlHelper, object htmlAttributes) {
            return htmlHelper.BeginFormAntiForgeryPost(htmlHelper.ViewContext.HttpContext.Request.Url.PathAndQuery, FormMethod.Post, new RouteValueDictionary(htmlAttributes));
        }
    }
}