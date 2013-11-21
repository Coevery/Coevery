namespace Coevery.Core.Navigation.ViewModels {
    public class PositionTreeModel {
        public int? ParentId { get; set; }
        public string ParentPosition { get; set; }
        public int Level { get; set; }
        public int Order { get; set; }
        public bool IsLeaf { get; set; }
        public bool Expanded { get; set; }
    }
}