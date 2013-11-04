using System.Collections.Generic;
using Coevery.ContentManagement.Records;
using Coevery.Data.Conventions;

namespace Coevery.Entities.Models {
    public class EntityMetadataRecord : ContentPartVersionRecord {
        public EntityMetadataRecord() {
            FieldMetadataRecords = new List<FieldMetadataRecord>();
        }

        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }

        [StringLengthMax]
        public virtual string Settings { get; set; }

        [CascadeAllDeleteOrphan]
        public virtual IList<FieldMetadataRecord> FieldMetadataRecords { get; set; }
    }
}