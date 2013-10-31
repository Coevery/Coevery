using Coevery.Events;
using Coevery.Orchard.Projections.Descriptors.SortCriterion;

namespace Coevery.Orchard.Projections.Services {
    public interface ISortCriterionProvider : IEventHandler {
        void Describe(DescribeSortCriterionContext describe);
    }
}