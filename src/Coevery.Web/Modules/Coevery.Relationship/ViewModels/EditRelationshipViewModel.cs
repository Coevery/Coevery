using System.Collections.Generic;
using System.Web.Mvc;

namespace Coevery.Relationship.ViewModels {
    public class EditRelationshipViewModel {
        public IEnumerable<SelectListItem> Links { get; set; }
        public string[] SelectedIds { get; set; }
        public string DisplayName { get; set; }
    }
}