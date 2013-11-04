using Coevery.ContentManagement.Records;

namespace Coevery.Core.Common.Models {
    public class IdentityPartRecord : ContentPartRecord {
        public virtual string Identifier { get; set; }
    }
}