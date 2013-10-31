using System.Collections.Generic;
using System.Reflection;

namespace Coevery.Environment {
    public interface ICoeveryFrameworkAssemblies : IDependency {
        IEnumerable<AssemblyName> GetFrameworkAssemblies();
    }

    public class DefaultCoeveryFrameworkAssemblies : ICoeveryFrameworkAssemblies {
        public IEnumerable<AssemblyName> GetFrameworkAssemblies() {
            return typeof (IDependency).Assembly.GetReferencedAssemblies();
        }
    }
}
