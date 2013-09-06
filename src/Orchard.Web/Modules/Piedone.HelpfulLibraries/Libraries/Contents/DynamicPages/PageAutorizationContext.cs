using Orchard.ContentManagement;
using Orchard.Environment.Extensions;
using Orchard.Security;

namespace Piedone.HelpfulLibraries.Contents.DynamicPages
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public class PageAutorizationContext : PageContext
    {
        public IUser User { get; private set; }
        public bool Granted { get; set; }

        public PageAutorizationContext(IContent page, string group, IUser user)
            : base(page, group)
        {
            User = user;
            Granted = false;
        }
    }
}
