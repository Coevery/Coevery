using System;
using Coevery.ContentManagement.Records;

namespace Coevery.Common.Models {
    public class CoeveryCommonPartRecord : ContentPartRecord {
        public virtual int OwnerId { get; set; }
        public virtual int ModifierId { get; set; }
        public virtual ContentItemRecord Container { get; set; }
        public virtual DateTime? CreatedUtc { get; set; }
        //public virtual DateTime? PublishedUtc { get; set; }
        public virtual DateTime? ModifiedUtc { get; set; }
    }
}