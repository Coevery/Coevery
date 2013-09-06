using System.Web.Mvc;
using Coevery.Relationship.Fields;

namespace Coevery.Relationship.Models
{
    public class ReferenceFieldViewModel
    {
        public ReferenceField Field { get; set; }
        public SelectList ItemList { get; set; }
        public int? ContentId { get; set; }
    }
}
