using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Records;

namespace Coevery.Relationship.Records {
    public class ManyToManyEntityMapRecord {
        public virtual int Id { get; set; }
        public virtual ContentItemRecord PrimaryEntry { get; set; }
        public virtual ContentItemRecord RelatedEntry { get; set; }
    }
}