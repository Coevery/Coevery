namespace Coevery.Core.ViewModels {
    public class ListQueryModel {
        public int PageSize { get; set; }
        public int Page { get; set; }
        public int ViewId { get; set; }
        public int FilterGroupId { get; set; }
        public FilterData[] Filters { get; set; }
    }
}