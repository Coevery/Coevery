using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using Coevery.ContentManagement.Records;
using Coevery.Data.Conventions;

namespace Coevery.Perspectives.Models
{
    public class PerspectivePartRecord : ContentPartRecord
    {
        [StringLength(255)]
        public virtual string Title { get; set; }

        [StringLengthMax]
        public virtual string Description { get; set; }

        public virtual int Position { get; set; }
        public virtual int CurrentLevel { get; set; }
    }
}
