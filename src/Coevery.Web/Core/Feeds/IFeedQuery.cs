using Coevery.Core.Feeds.Models;

namespace Coevery.Core.Feeds {
    public interface IFeedQuery {
        void Execute(FeedContext context);
    }
}