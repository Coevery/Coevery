using System.Text;
using Orchard;
using Piedone.Combinator.Models;

namespace Piedone.Combinator.Services
{
    public interface IResourceProcessingService : IDependency
    {
        void ProcessResource(CombinatorResource resource, StringBuilder combinedContent, ICombinatorSettings settings);
    }
}
