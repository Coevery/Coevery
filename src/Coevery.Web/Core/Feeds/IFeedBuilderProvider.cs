using Coevery.Core.Feeds.Models;

namespace Coevery.Core.Feeds {
    public interface IFeedBuilderProvider : IDependency {
        FeedBuilderMatch Match(FeedContext context);
    }
    
    public class FeedBuilderMatch {
        public int Priority { get; set; }
        public IFeedBuilder FeedBuilder { get; set; }
    }
}
