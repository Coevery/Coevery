using System.Collections.Generic;
using System.Web.Mvc;
using Coevery.Relationship.Records;

namespace Coevery.Relationship.Models {
    public class OneToManyRelationshipModel : RelationshipModel {
        public string RelatedListLabel { get; set; }
        public bool ShowRelatedList { get; set; }
        public OneToManyDeleteOption DeleteOption { get; set; }
        public string[] ColumnFieldList { get; set; }
        public IEnumerable<SelectListItem> Fields { get; set; }

        //LookupField related
        public string HelpText { get; set; }
        public bool Required { get; set; }
        public bool AlwaysInLayout { get; set; }
        public bool IsAudit { get; set; }
        public bool DisplayAsLink { get; set; }
        public string FieldLabel { get; set; }
        public string FieldName { get; set; }
    }
}