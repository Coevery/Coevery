using System.Web.Mvc;
using Coevery.Fields.Fields;


namespace Coevery.Fields.ViewModels
{
    public class ReferenceFieldViewModel
    {
        public ReferenceField Field { get; set; }
        public SelectList ItemList { get; set; }
        public int? ContentId { get; set; }
    }
}
