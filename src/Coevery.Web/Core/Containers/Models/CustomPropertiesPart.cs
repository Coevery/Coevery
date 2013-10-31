using Coevery.ContentManagement;
using Coevery.ContentManagement.Records;

namespace Coevery.Core.Containers.Models {
    public class CustomPropertiesPart : ContentPart<CustomPropertiesPartRecord> {
    }

    public class CustomPropertiesPartRecord : ContentPartRecord {
        public virtual string CustomOne { get; set; }
        public virtual string CustomTwo { get; set; }
        public virtual string CustomThree { get; set; }
    }
}
