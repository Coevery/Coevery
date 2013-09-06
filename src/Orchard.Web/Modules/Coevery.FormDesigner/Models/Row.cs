using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coevery.FormDesigner.Models {
    public class Row {
        public Column[] Columns { get; set; }
        public bool IsMerged { get; set; }
    }
}