using Orchard.ContentManagement;
using Orchard.Projections.Models;

namespace Coevery.Projections.Models {
    public class ListViewPart : ContentPart<ListViewPartRecord>
    {
        public string VisableTo
        {
            get { return Record.VisableTo; }
            set { Record.VisableTo = value; }
        }

        public int PageRowCount
        {
            get { return Record.PageRowCount; }
            set { Record.PageRowCount = value; }
        }

        public int QueryPartRecord_id
        {
            get { return Record.QueryPartRecord_id; }
            set { Record.QueryPartRecord_id = value; }
        }
    }
}