using System;
using System.ComponentModel.DataAnnotations;


namespace Coevery.Leads.Models
{
    public class LeadRecord
    {
        [Key]
        public virtual Int32 Id { get; set; }

        [Required]
        public virtual string Topic { get; set; }
        [Required]
        public virtual string FirstName { get; set; }
        [Required]
        public virtual string LastName { get; set; }

        public virtual int StatusCode { get; set; }
    }
}