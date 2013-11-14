using System.ComponentModel.DataAnnotations;
using Coevery.ContentManagement;
using Coevery.Data.Conventions;

namespace Coevery.Perspectives.Models
{
    public class PerspectivePart : ContentPart<PerspectivePartRecord> {
        [Required]
        [StringLength(255)]
        public string Title
        {
            get { return Record.Title; }
            set { Record.Title = value; }
        }

        [StringLengthMax]
        public string Description {
            get { return Record.Description; }
            set { Record.Description = value; }
        }

        public int Position
        {
            get { return Record.Position; }
            set { Record.Position = value; }
        }

        public int CurrentLevel {
            get { return Record.CurrentLevel; }
            set { Record.CurrentLevel = value; }
        }
    }
}