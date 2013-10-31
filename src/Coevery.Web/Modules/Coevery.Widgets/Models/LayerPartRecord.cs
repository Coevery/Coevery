using Coevery.ContentManagement.Records;

namespace Coevery.Widgets.Models {
    public class LayerPartRecord : ContentPartRecord {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string LayerRule { get; set; }
    }
}