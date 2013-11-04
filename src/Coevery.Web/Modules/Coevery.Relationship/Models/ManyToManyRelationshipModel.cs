using System.Collections.Generic;
using System.Web.Mvc;

namespace Coevery.Relationship.Models {
    public class ManyToManyRelationshipModel : RelationshipModel {
        public string[] PrimaryColumnList { get; set; }
        public IEnumerable<SelectListItem> PrimaryFields { get; set; }
        public string[] RelatedColumnList { get; set; }
        public IEnumerable<SelectListItem> RelatedFields { get; set; }
        public string RelatedListLabel { get; set; }
        public bool ShowRelatedList { get; set; }
        public string PrimaryListLabel { get; set; }
        public bool ShowPrimaryList { get; set; }
    }
}