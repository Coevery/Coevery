using Coevery.Core.Feeds.Models;

namespace Coevery.Core.Feeds {
    public interface IFeedQueryProvider : IDependency {
        FeedQueryMatch Match(FeedContext context);
    }

    public class FeedQueryMatch {
        public int Priority { get; set; }
        public IFeedQuery FeedQuery { get; set; }
    }
}
