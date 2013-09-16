using Orchard.ContentManagement.Records;
using Orchard.Projections.Models;

namespace Coevery.Projections.Models
{
    public class ListViewPartRecord : ContentPartRecord
    {
        public virtual string VisableTo { get; set; }
        public virtual string ItemContentType { get; set; }
        public virtual bool IsDefault { get; set; }
    }
}