using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Coevery.Relationship.Models {
    public class RelationshipViewModel<T> {
        public SelectListItem[] EntityList { get; set; }
        public T RelationshipRecord { get; set; }
    }
}