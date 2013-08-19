using System.Collections.Generic;
using Coevery.Taxonomies.Settings;

namespace Coevery.Taxonomies.ViewModels {
    public class TaxonomyFieldViewModel {
        public int TaxonomyId { get; set; }
        public string Name { get; set; }
        public TaxonomyFieldSettings Settings { get; set; }
        public IList<TermEntry> Terms { get; set; }
        public int SingleTermId { get; set; }
    }
}