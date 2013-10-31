using Coevery.ContentManagement;

namespace Coevery.Core.Common.Models {
    public class IdentityPart : ContentPart<IdentityPartRecord> {
        public string Identifier {
            get { return Record.Identifier; }
            set { Record.Identifier = value; }
        }
    }
}
