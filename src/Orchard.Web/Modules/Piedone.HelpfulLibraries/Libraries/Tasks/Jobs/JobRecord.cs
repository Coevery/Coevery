using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Data.Conventions;
using Orchard.Environment.Extensions;

// Records should be in the Models namespace...
namespace Piedone.HelpfulLibraries.Models
{
    [OrchardFeature("Piedone.HelpfulLibraries.Tasks.Jobs")]
    public class JobRecord
    {
        public virtual int Id { get; set; }
        public virtual string Industry { get; set; }
        [StringLengthMax]
        public virtual string ContextDefinion { get; set; }
        public virtual int Priority { get; set; }
    }
}
