using Coevery.ContentManagement;
using Coevery.Events;
using Coevery.Projections.Models;

namespace Coevery.Projections.Services {
    public interface IQueryCriteriaProvider : IEventHandler {
        void Apply(QueryContext context);
    }

    public class QueryContext {
        public QueryPartRecord QueryPartRecord { get; set; }
        public IHqlQuery Query { get; set; }
        public string ContentTypeName { get; set; }
    }
}