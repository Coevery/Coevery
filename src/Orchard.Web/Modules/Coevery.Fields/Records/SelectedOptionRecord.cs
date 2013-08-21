using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Data.Conventions;

namespace Coevery.Fields.Records {
    public class SelectedOptionRecord {
        public virtual int Id { get; set; } 
        public virtual OptionItemRecord OptionItem { get; set; }
        public virtual SelectedOptionSetRecord SelectedOptionSetRecord { get; set; }
    }
}