using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;

namespace Coevery.OptionSet.Models {
    /// <summary>
    /// This Content Part is used to create a link to TermContentItem records, so
    /// that the Content Manager can query them. It will be attached dynamically whenever
    /// a TaxonomyField is found on a Content Type
    /// </summary>
    public class OptionItemContainerPart : ContentPart<OptionItemContainerPartRecord> {
        public IList<OptionItemContentItem> OptionItems { get { return Record.OptionItems; } }
        internal LazyField<IEnumerable<OptionItemContentItemPart>> _optionItemParts = new LazyField<IEnumerable<OptionItemContentItemPart>>();
        public IEnumerable<OptionItemContentItemPart> OptionItemParts { get { return _optionItemParts.Value; } }
    }
}