using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Coevery.Relationship.Models {
    public class RelationshipModel {
        public SelectListItem[] EntityList { get; set; }
        public string Name { get; set; }
        public string PrimaryEntity { get; set; }
        public string RelatedEntity { get; set; }
        public bool IsCreate { get; set; }
    }
}