using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Relationship.Records;

namespace Coevery.Relationship.Models {
    public class ManyToManyRelationshipModel: RelationshipModel {
        public string[] PrimaryColumnList { get; set; }
        public string[] RelatedColumnList { get; set; }
        public string RelatedListLabel { get; set; }
        public bool ShowRelatedList { get; set; }
        public string PrimaryListLabel { get; set; }
        public bool ShowPrimaryList { get; set; }
    }
}