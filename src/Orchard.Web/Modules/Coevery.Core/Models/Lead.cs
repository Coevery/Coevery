using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace DynamicTypes.Models
{
    public class Lead : ContentPart<LeadRecord>
    {
        [Required]
        public string Topic
        {
            get { return Record.Topic; }
            set { Record.Topic = value; }
        }

        [Required]
        public string FirstName
        {
            get { return Record.FirstName; }
            set { Record.FirstName = value; }
        }

        [Required]
        public string LastName
        {
            get { return Record.LastName; }
            set { Record.LastName = value; }
        }

        public int StatusCode
        {
            get { return Record.StatusCode; }
            set { Record.StatusCode = value; }
        }
    }
}