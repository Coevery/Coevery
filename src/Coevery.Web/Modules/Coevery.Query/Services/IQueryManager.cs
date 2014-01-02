using System.Collections.Generic;
using Coevery.ContentManagement;

namespace Coevery.Query.Services {
    public interface IQueryManager : IDependency {
        IEnumerable<ContentItem> GetContentItems(int queryId, string contentTypeName, int skip = 0, int count = 0);
        int GetCount(int queryId);
    }
}