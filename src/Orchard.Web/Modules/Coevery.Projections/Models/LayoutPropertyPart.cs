using Orchard.ContentManagement;
using Orchard.Projections.Models;

namespace Coevery.Projections.Models {
    public class LayoutPropertyPart : ContentPart<LayoutPropertyRecord>
    {

        public virtual string VisableTo
        {
            get { return Record.VisableTo; }
            set { Record.VisableTo = value; }
        }

        public virtual int PageRowCount
        {
            get { return Record.PageRowCount; }
            set { Record.PageRowCount = value; }
        }

        public virtual int QueryPartRecord_id
        {
            get { return Record.QueryPartRecord_id; }
            set { Record.QueryPartRecord_id = value; }
        }
    }
}