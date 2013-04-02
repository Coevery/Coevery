using System.ComponentModel.DataAnnotations;
using Orchard.WebApi.Common;

namespace DynamicTypes.Models
{
    public class LeadDto : IDto<Lead>
    {
        public LeadDto() { }

        public LeadDto(Lead item)
        {
            Id = item.Id;
            Topic = item.Topic;
            FirstName = item.FirstName;
            LastName = item.LastName;
            StatusCode = item.StatusCode;
        }

        [Key]
        public int Id { get; set; }

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
            get { return Id; }
            set { Id = value; }
        }
    }
}