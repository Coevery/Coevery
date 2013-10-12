using Orchard.ContentManagement;
using Orchard.Core.Settings.Metadata.Records;

namespace Coevery.Core.Models {
    public class ModuleMenuItemPart : ContentPart<ModuleMenuItemPartRecord> {
        public ContentTypeDefinitionRecord ContentTypeDefinitionRecord {
            get { return Record.ContentTypeDefinitionRecord; }
            set { Record.ContentTypeDefinitionRecord = value; }
        }

        public string IconClass {
            get { return Record.IconClass; }
            set { Record.IconClass = value; }
        }
    }
}