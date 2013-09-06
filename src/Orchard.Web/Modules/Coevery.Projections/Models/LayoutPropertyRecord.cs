using Orchard.ContentManagement.Records;
using Orchard.Projections.Models;

namespace Coevery.Projections.Models
{
    public class LayoutPropertyRecord : ContentPartRecord
    {
        public virtual string VisableTo { get; set; }
        public virtual int PageRowCount { get; set; }
        public virtual int QueryPartRecord_id { get; set; }
    }
}