using Coevery.Core.ViewModels;

namespace Coevery.Projections.ViewModels {
    public class FilterViewModel {
        public string Title { get; set; }
        public FilterData[] Filters { get; set; }
    }
}