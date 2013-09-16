using Orchard.ContentManagement;
using Orchard.Projections.Models;

namespace Coevery.Projections.Models {
    public class ListViewPart : ContentPart<ListViewPartRecord> {
        public string VisableTo {
            get { return Record.VisableTo; }
            set { Record.VisableTo = value; }
        }

        public string ItemContentType {
            get { return Record.ItemContentType; }
            set { Record.ItemContentType = value; }
        }

        public bool IsDefault {
            get { return Record.IsDefault; }
            set { Record.IsDefault = value; }
        }
    }
}