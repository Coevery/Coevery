using Orchard.ContentManagement.Records;

namespace Coevery.Taxonomies.Models {
    public class TaxonomyPartRecord : ContentPartRecord {
        public virtual string TermTypeName { get; set; }
        public virtual bool IsInternal { get; set; }
    }
}
