using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coevery.Entities.Models {
    public class FieldWithEntityInfoModel {
        public int EntityId { get; set; }
        public string EntityName { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; }
    }
}