using System.Collections.Generic;
using Coevery.Entities.Services;
using Coevery.Taxonomies.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;

namespace Coevery.Taxonomies.Fields {
    /// <summary>
    /// This field has not state, as all terms are saved using <see cref="TermContentItem"/>
    /// </summary>
    [CustomField]
    public class TaxonomyField : ContentField {
        internal LazyField<IEnumerable<TermPart>> TermsField { get; set; }

        public TaxonomyField() {
            TermsField = new LazyField<IEnumerable<TermPart>>();
        }

        /// <summary>
        /// Gets the Terms associated with this field
        /// </summary>
        public IEnumerable<TermPart> Terms {
            get { return TermsField.Value; }
        }
    }
}