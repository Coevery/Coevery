using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data.Conventions;

namespace Coevery.Fields.Records {
    public class OptionSetRecord {
        public virtual int Id { get; set; }
        public string FieldName { get; set; }

        [CascadeAllDeleteOrphan]
        public virtual IList<OptionItemRecord> OptionItemRecords { get; set; }
    }
}