using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Relationship.Records;

namespace Coevery.Relationship.Models {
    public class OneToManyRelationshipModel : RelationshipModel {
        public string RelatedListLabel { get; set; }
        public bool ShowRelatedList { get; set; }
        public OneToManyDeleteOption DeleteOption { get; set; }
        public string[] ColumnFieldList { get; set; }

    }
}