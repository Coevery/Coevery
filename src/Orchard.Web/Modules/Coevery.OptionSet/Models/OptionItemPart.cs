using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Title.Models;

namespace Coevery.OptionSet.Models {
    public class OptionItemPart : ContentPart<OptionItemPartRecord> {
        public string Name {
            get { return this.As<TitlePart>().Title; }
            set { this.As<TitlePart>().Title = value; }
        }

        public IContent Container {
            get { return this.As<ICommonPart>().Container; }
            set { this.As<ICommonPart>().Container = value; }
        }

        public int OptionSetId {
            get { return Record.OptionSetId; }
            set { Record.OptionSetId = value; }
        }

        public bool Selectable {
            get { return Record.Selectable; }
            set { Record.Selectable = value; }
        }

        public int Weight {
            get { return Record.Weight; }
            set { Record.Weight = value; }
        }

        public static IEnumerable<OptionItemPart> Sort(IEnumerable<OptionItemPart> terms) {
            var list = terms.ToList();
            return list;
        }
    }
}