namespace Coevery.Core.ViewModels {
    public class ListQueryModel {
        public int Rows { get; set; }
        public int Page { get; set; } 
        public string Sidx { get; set; }
        public string Sord { get; set; }
        public int ViewId { get; set; }
        public int FilterGroupId { get; set; }
        public FilterData[] Filters { get; set; }
        /*Relationship related*/
        public bool IsRelationList { get; set; }
        public int CurrentItem { get; set; }
        public string RelationId { get; set; }
        public string RelationType { get; set; }
    }
}