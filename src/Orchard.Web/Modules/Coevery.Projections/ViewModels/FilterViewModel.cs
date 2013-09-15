using Coevery.Core.ViewModels;

namespace Coevery.Projections.ViewModels {
    public class FilterViewModel {
        public int Id { get; set; }
        public int FilterGroupId { get; set; }
        public string Title { get; set; }
        public FilterData[] Filters { get; set; }
    }
}