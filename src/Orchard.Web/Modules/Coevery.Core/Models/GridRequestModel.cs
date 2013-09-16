using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coevery.Core.Models {
    public class GridRequestModel {
        public int Page { get; set; }
        public int Rows { get; set; }
        public bool _search { get; set; }
        public int Nd { get; set; }
        public string Sidx { get; set; }
        public string Sord { get; set; }
    }
}