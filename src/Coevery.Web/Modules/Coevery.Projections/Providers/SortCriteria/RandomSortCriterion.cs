using Coevery.Localization;
using Coevery.Projections.Descriptors.SortCriterion;
using Coevery.Projections.Services;

namespace Coevery.Projections.Providers.SortCriteria {
    public class RandomSortCriterion : ISortCriterionProvider {

        public RandomSortCriterion() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeSortCriterionContext describe) {
            describe.For("General", T("General"), T("General sort criteria"))
                .Element("Random", T("Random"), T("Sorts the results randomly."),
                         context => context.Query.OrderBy(alias => alias.ContentItem(), order => order.Random()),
                         context => T("Random order")
                );
        }
    }
}