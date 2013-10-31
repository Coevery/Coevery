using Coevery.ContentManagement;
using Coevery.Data.Conventions;

namespace Coevery.Core.Settings.Models {
    public sealed class SiteSettings2Part : ContentPart<SiteSettings2PartRecord> {
        [StringLengthMax]
        public string BaseUrl {
            get { return Record.BaseUrl; }
            set { Record.BaseUrl = value; }
        }
    }
}