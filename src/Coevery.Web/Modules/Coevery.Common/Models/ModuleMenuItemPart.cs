using Coevery.ContentManagement;
using Coevery.Core.Settings.Metadata.Records;

namespace Coevery.Common.Models {
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