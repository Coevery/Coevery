namespace Coevery.Common.ViewModels {
    public class ListQueryModel {
        public int Rows { get; set; }
        public int Page { get; set; } 
        public string Sidx { get; set; }
        public string Sord { get; set; }
        public int ViewId { get; set; }
    }
}