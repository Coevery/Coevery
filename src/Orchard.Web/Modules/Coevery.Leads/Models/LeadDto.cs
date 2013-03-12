using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Coevery.Leads.Models
{
    public class LeadDto
    {
        public LeadDto() { }

        public LeadDto(LeadRecord item)
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

        public LeadRecord ToEntity()
        {
            return new LeadRecord
            {
                Id = LeadId,
                Topic = Topic,
                FirstName = FirstName,
                LastName = LastName,
                StatusCode = StatusCode
            };
        }
    }
}