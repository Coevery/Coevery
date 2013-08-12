using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Relationship.Records;

namespace Coevery.Relationship.Models {
    public class ManyToManyRelationshipModel {
        public ManyToManyRelationshipRecord ManyToManyRelationship { get; set; }
        public Dictionary<int,bool> ColumnFieldList { get; set; }

        public ManyToManyRelationshipModel() {
            ManyToManyRelationship = new ManyToManyRelationshipRecord();
            ColumnFieldList = new Dictionary<int, bool>();
        }
    }
}