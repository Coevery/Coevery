using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Orchard.WebApi.Common;

namespace Coevery.Leads.Models
{
    public class LeadDto : IDto<Lead>
    {
        public LeadDto() { }

        public LeadDto(Lead item)
        {
            LeadId = item.Id;
            Topic = item.Topic;
            FirstName = item.FirstName;
            LastName = item.LastName;
            StatusCode = item.StatusCode;
        }

        [Key]
        public int LeadId { get; set; }

        [Required]
        public string Topic { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public int StatusCode { get; set; }

        public void UpdateEntity(Lead lead)
        {
            lead.Topic = Topic;
            lead.FirstName = FirstName;
            lead.LastName = LastName;
            lead.StatusCode = StatusCode;
        }

        public int ContentId
        {
            get { return LeadId; }
            set { LeadId = value; }
        }
    }
}