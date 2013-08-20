using Orchard;
using Orchard.ContentManagement;

namespace Coevery.Core.Services {
    public interface ICoeveryCommonService : IDependency {
        void WeldCommonPart(string typeName);
    }
}