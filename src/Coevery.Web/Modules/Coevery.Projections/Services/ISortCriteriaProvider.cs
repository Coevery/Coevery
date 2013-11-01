using Coevery.Events;
using Coevery.Projections.Descriptors.SortCriterion;

namespace Coevery.Projections.Services {
    public interface ISortCriterionProvider : IEventHandler {
        void Describe(DescribeSortCriterionContext describe);
    }
}