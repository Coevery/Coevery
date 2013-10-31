using Coevery.ContentManagement.Records;
using Coevery.Data.Conventions;

namespace Coevery.Core.Common.Models {
    public class BodyPartRecord : ContentPartVersionRecord {
        [StringLengthMax]
        public virtual string Text { get; set; }

        public virtual string Format { get; set; }
    }
}