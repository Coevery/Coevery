using Orchard.ContentManagement.Records;

namespace DynamicTypes.Models
{
    public class LeadRecord : ContentPartRecord
    {
        public virtual string Topic { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual int StatusCode { get; set; }
    }
}