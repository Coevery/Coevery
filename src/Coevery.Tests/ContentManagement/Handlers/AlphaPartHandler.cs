using Coevery.ContentManagement.Handlers;
using Coevery.Tests.ContentManagement.Models;

namespace Coevery.Tests.ContentManagement.Handlers {
    public class AlphaPartHandler : ContentHandler {
        public AlphaPartHandler() {
            Filters.Add(new ActivatingFilter<AlphaPart>("alpha"));

            OnGetDisplayShape<AlphaPart>((ctx, part) => ctx.Shape.Zones["Main"].Add(part, "3"));
        }
    }
}
