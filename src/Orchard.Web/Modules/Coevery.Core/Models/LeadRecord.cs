using Orchard.ContentManagement.Records;

namespace Coevery.Leads.Models
{
    public class LeadRecord : ContentPartRecord
    {
        public virtual string Topic { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual int StatusCode { get; set; }
    }
}