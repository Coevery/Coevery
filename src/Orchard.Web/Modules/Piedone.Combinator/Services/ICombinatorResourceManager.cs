using Orchard;
using Piedone.Combinator.Models;

namespace Piedone.Combinator.Services
{
    public interface ICombinatorResourceManager :  IDependency
    {
        CombinatorResource ResourceFactory(ResourceType type);
        void DeserializeSettings(string serialization, CombinatorResource resource);
        string SerializeResourceSettings(CombinatorResource resource);
    }
}
