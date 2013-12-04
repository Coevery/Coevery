using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Coevery.Data.Conventions;

namespace Coevery.Projections.Models {
    public class LayoutGroupRecord {
        public virtual int Id { get; set; }
        public virtual int Position { get; set; }

        [StringLength(64)]
        public virtual string Sort { get; set; }

        // Parent property
        public virtual LayoutRecord LayoutRecord { get; set; }

        [CascadeAllDeleteOrphan, Aggregate]
        public virtual PropertyRecord GroupProperty { get; set; }
    }
}