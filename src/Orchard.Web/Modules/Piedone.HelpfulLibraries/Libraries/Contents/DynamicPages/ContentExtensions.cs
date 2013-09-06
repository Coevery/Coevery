using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Contents.DynamicPages
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public static class ContentExtensions
    {
        /// <summary>
        /// Checks whether a content item is a page with the specified name
        /// </summary>
        public static bool IsPage(this IContent page, string pageName, string group)
        {
            return page.ContentItem.ContentType.EndsWith(ContentManagerExtensions.CreatePageName(null, pageName, group));
        }
    }
}
