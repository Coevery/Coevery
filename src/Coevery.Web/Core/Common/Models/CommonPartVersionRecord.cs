using System;
using Coevery.ContentManagement.Records;

namespace Coevery.Core.Common.Models {
    public class CommonPartVersionRecord : ContentPartVersionRecord {
        public virtual DateTime? CreatedUtc { get; set; }
        public virtual DateTime? PublishedUtc { get; set; }
        public virtual DateTime? ModifiedUtc { get; set; }
    }
}