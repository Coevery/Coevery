using System;
using Coevery.ContentManagement;

namespace Coevery.Tasks.Indexing {
    public interface IIndexingTask {
        ContentItem ContentItem { get; }
        DateTime? CreatedUtc { get; }
    }
}
