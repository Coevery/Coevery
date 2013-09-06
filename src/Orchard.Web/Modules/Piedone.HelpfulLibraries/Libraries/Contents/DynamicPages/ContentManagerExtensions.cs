using System;
using Orchard.ContentManagement;
using Orchard.Environment.Extensions;

namespace Piedone.HelpfulLibraries.Contents.DynamicPages
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public static class ContentManagerExtensions
    {
        /// <summary>
        /// Creates a new page content items
        /// </summary>
        /// <param name="pageName">Name of the page</param>
        /// <param name="group">String id of the group the page belongs to. Use this to distinct between a set of pages.</param>
        /// <param name="initializer">Delegate to run immediately after the page item is created</param>
        /// <param name="eventHandler">Page event handler</param>
        public static IContent NewPage(this IContentManager contentManager, string pageName, string group, Action<IContent> initializer, IPageEventHandler eventHandler)
        {
            var page = contentManager.New(contentManager.CreatePageName(pageName, group));

            initializer(page);

            var context = new PageContext(page, group);
            eventHandler.OnPageInitializing(context);
            eventHandler.OnPageInitialized(context);

            return page;
        }

        /// <summary>
        /// Creates a new page content items
        /// </summary>
        /// <param name="pageName">Name of the page</param>
        /// <param name="group">String id of the group the page belongs to. Use this to distinct between a set of pages.</param>
        /// <param name="eventHandler">Page event handler</param>
        public static IContent NewPage(this IContentManager contentManager, string pageName, string group, IPageEventHandler eventHandler)
        {
            return contentManager.NewPage(pageName, group, (content) => { }, eventHandler);
        }

        /// <summary>
        /// Compiles a conventional name for a page
        /// </summary>
        /// <param name="pageName">Name of the page</param>
        /// <param name="group">String id of the group the page belongs to. Use this to distinct between a set of pages.</param>
        public static string CreatePageName(this IContentManager contentManager, string pageName, string group)
        {
            return group + pageName + "Page";
        }

        /// <summary>
        /// Builds the display shape of the page
        /// </summary>
        /// <param name="page">The page item</param>
        /// <param name="displayType">The display type (e.g. Summary, Detail) to use</param>
        /// <param name="groupId">Id of the display group (stored in the content item's metadata)</param>
        public static dynamic BuildPageDisplay(this IContentManager contentManager, IContent page, string displayType = "", string groupId = "")
        {
            var shape = contentManager.BuildDisplay(page, displayType, groupId);
            shape.Metadata.Wrappers.Add("PageWrapper_" + page.ContentItem.ContentType.Replace('.', '_'));
            return shape;
        }
    }
}
