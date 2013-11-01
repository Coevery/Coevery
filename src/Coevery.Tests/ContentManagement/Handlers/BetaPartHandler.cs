using Coevery.ContentManagement.Handlers;
using Coevery.Tests.ContentManagement.Models;

namespace Coevery.Tests.ContentManagement.Handlers {
    public class BetaPartHandler : ContentHandler {
        public BetaPartHandler() {
            Filters.Add(new ActivatingFilter<BetaPart>("beta"));
        }
    }
}
