using System.Collections.Generic;
using System.Text;
using Orchard;

namespace Coevery.Metadata.Services
{
    public interface ITypeDeployService : IDependency {
        bool DeployType(string name);
    }
}
