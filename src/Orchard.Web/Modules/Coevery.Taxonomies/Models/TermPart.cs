using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Title.Models;

namespace Coevery.Taxonomies.Models {
    public class TermPart : ContentPart<TermPartRecord> {
        public string Name {
            get { return this.As<TitlePart>().Title; }
            set { this.As<TitlePart>().Title = value; }
        }

        public IContent Container {
            get { return this.As<ICommonPart>().Container; }
            set { this.As<ICommonPart>().Container = value; }
        }

        public int TaxonomyId {
            get { return Record.TaxonomyId; }
            set { Record.TaxonomyId = value; }
        }

        public bool Selectable {
            get { return Record.Selectable; }
            set { Record.Selectable = value; }
        }

        public int Weight {
            get { return Record.Weight; }
            set { Record.Weight = value; }
        }

        public static IEnumerable<TermPart> Sort(IEnumerable<TermPart> terms) {
            var list = terms.ToList();
            return list;
        }
    }
}