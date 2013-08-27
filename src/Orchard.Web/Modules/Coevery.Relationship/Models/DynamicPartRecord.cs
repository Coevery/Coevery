using System.Collections.Generic;
using Orchard.ContentManagement.Records;

namespace Coevery.Relationship.Models {
    //public interface IDynamicPartRecord {
    //    IList<IContentLinkRecord> Links { get; set; }
    //}

    public abstract class DynamicPartRecord<TContentLinkRecord> : ContentPartRecord
        where TContentLinkRecord : IContentLinkRecord {
        protected DynamicPartRecord() {
            Links = new List<TContentLinkRecord>();
        }

        public virtual IList<TContentLinkRecord> Links { get; set; }
    }
}