using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Core.Title.Models;

namespace Coevery.OptionSet.Models {
    public class OptionSetPart : ContentPart<OptionSetPartRecord> {
        internal LazyField<IEnumerable<OptionItemPart>> OptionItemsField = new LazyField<IEnumerable<OptionItemPart>>();
        public IEnumerable<OptionItemPart> OptionItems { get { return OptionItemsField.Value; } }

        public string Name {
            get { return this.As<TitlePart>().Title; }
            set { this.As<TitlePart>().Title = value; }
        }

        public string TermTypeName {
            get { return Record.TermTypeName; }
            set { Record.TermTypeName = value; }
        }

        public bool IsInternal {
            get { return Record.IsInternal; }
            set { Record.IsInternal = value; }
        }

    }
}
