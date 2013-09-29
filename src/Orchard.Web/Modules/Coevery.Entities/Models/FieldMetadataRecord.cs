using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data.Conventions;

namespace Coevery.Entities.Models {
    public class FieldMetadataRecord {
        public virtual int Id { get; set; }
        public virtual int OriginalId { get; set; }
        public virtual ContentFieldDefinitionRecord ContentFieldDefinitionRecord { get; set; }
        public virtual string Name { get; set; }
        public virtual EntityMetadataRecord EntityMetadataRecord { get; set; }

        [StringLengthMax]
        public virtual string Settings { get; set; }
    }
}