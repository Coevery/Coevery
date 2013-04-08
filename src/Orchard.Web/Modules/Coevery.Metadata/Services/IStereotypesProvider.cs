using Orchard;
using System.Collections.Generic;

namespace Coevery.Metadata.Services
{
    public interface IStereotypesProvider : IDependency
    {
        IEnumerable<StereotypeDescription> GetStereotypes();
    }

    public class StereotypeDescription
    {
        public string Stereotype { get; set; }
        public string DisplayName { get; set; }
    }
}