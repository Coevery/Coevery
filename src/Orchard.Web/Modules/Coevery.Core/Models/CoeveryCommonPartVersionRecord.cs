using System;
using Orchard.ContentManagement.Records;

namespace Coevery.Core.Models {
    public class CoeveryCommonPartVersionRecord : ContentPartVersionRecord {
        public virtual DateTime? CreatedUtc { get; set; }
        //public virtual DateTime? PublishedUtc { get; set; }
        public virtual DateTime? ModifiedUtc { get; set; }
    }
}