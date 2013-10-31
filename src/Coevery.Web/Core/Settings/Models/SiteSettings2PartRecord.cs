using Coevery.ContentManagement.Records;
using Coevery.Data.Conventions;

namespace Coevery.Core.Settings.Models {
    public class SiteSettings2PartRecord : ContentPartRecord {
        [StringLengthMax]
        public virtual string BaseUrl { get; set; }
    }
}