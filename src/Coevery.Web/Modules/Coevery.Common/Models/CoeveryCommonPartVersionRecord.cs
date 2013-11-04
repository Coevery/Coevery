using System;
using Coevery.ContentManagement.Records;

namespace Coevery.Common.Models {
    public class CoeveryCommonPartVersionRecord : ContentPartVersionRecord {
        public virtual DateTime? CreatedUtc { get; set; }
        //public virtual DateTime? PublishedUtc { get; set; }
        public virtual DateTime? ModifiedUtc { get; set; }
    }
}