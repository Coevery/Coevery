using Coevery.ContentManagement.Handlers;

namespace Coevery.ContentManagement.Drivers {
    public class DriverResult {
        public virtual void Apply(BuildDisplayContext context) { }
        public virtual void Apply(BuildEditorContext context) { }
        
        public ContentPart ContentPart { get; set; }
        public ContentField ContentField { get; set; }
    }
}
