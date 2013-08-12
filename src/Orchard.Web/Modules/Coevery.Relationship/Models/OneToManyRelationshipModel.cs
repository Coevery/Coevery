using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Relationship.Records;

namespace Coevery.Relationship.Models {
    public class OneToManyRelationshipModel {
        public OneToManyRelationshipRecord OneToManyRelationship { get; set; }
        public string[] ColumnFieldList { get; set; }

        public OneToManyRelationshipModel() {
            OneToManyRelationship = new OneToManyRelationshipRecord();
            ColumnFieldList = null;
        }
    }
}