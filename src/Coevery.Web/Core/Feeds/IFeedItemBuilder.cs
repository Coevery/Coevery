using Coevery.Core.Feeds.Models;
using Coevery.Events;

namespace Coevery.Core.Feeds {
    public interface IFeedItemBuilder : IEventHandler {
        void Populate(FeedContext context);
    }
}
