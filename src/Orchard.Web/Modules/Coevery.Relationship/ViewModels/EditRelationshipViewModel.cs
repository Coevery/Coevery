using System.Collections.Generic;

namespace Coevery.Relationship.ViewModels {
    public class EditRelationshipViewModel {
        public IList<RelationshipEntry> Links { get; set; }
    }

    public class RelationshipEntry {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsChecked { get; set; }
    }
}