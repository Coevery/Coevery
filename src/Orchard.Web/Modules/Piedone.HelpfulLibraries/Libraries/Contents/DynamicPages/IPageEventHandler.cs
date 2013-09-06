using Orchard.ContentManagement;
using Orchard.Events;

namespace Piedone.HelpfulLibraries.Contents.DynamicPages
{
    /// <summary>
    /// Base interface for page event handlers
    /// </summary>
    public interface IPageEventHandler : IEventHandler
    {
        void OnPageInitializing(PageContext pageContext);
        void OnPageInitialized(PageContext pageContext);
        void OnPageBuilt(PageContext pageContext);
        void OnAuthorization(PageAutorizationContext authorizationContext);
    }
}