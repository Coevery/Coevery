using System.ComponentModel.DataAnnotations;
using Orchard.ContentManagement;

namespace Coevery.Opportunities.Models
{
    public class Opportunity : ContentPart<OpportunityRecord>
    {
        public string Name
        {
            get { return Record.Name; }
            set { Record.Name = value; }
        }

        public string Description
        {
            get { return Record.Description; }
            set { Record.Description = value; }
        }

        public int SourceLeadId
        {
            get { return Record.SourceLeadId; }
            set { Record.SourceLeadId = value; }
        }
    }
}