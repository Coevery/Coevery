using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;

namespace Coevery.Relationship.Models {
    public abstract class DynamicPrimaryPart<TPrimaryPartRecord, TContentLinkRecord> : ContentPart<TPrimaryPartRecord>
        where TPrimaryPartRecord : DynamicPartRecord<TContentLinkRecord>
        where TContentLinkRecord : IContentLinkRecord {
        public IEnumerable<ContentPartRecord> Links {
            get { return Record.Links.Select(r => r.RelatedPartRecord); }
        }
    }
}