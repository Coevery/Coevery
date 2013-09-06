using System;
using Orchard.Environment.Extensions;

namespace Piedone.Combinator.Models
{
    [OrchardFeature("Piedone.Combinator")]
    public class CombinedFileRecord
    {
        public virtual int Id { get; set; }
        public virtual int HashCode { get; set; }
        public virtual int Slice { get; set; }
        public virtual ResourceType Type { get; set; }
        public virtual DateTime? LastUpdatedUtc { get; set; }
        public virtual string Settings { get; set; }
    }
}