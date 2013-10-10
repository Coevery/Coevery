using System.Collections.Generic;
using Orchard.ContentManagement;

namespace Coevery.Entities.Models {
    public class EntityMetadataPart : ContentPart<EntityMetadataRecord> {
        public string Name {
            get { return Record.Name; }
            set { Record.Name = value; }
        }

        public string DisplayName {
            get { return Record.DisplayName; }
            set { Record.DisplayName = value; }
        }

        public IList<FieldMetadataRecord> FieldMetadataRecords {
            get { return Record.FieldMetadataRecords; }
        }
    }
}