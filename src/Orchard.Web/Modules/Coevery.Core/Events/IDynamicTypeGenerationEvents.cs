using System.Reflection.Emit;
using Orchard.Events;

namespace Coevery.Core.Events {
    public interface IDynamicTypeGenerationEvents : IEventHandler {
        void OnBuilded(ModuleBuilder moduleBuilder);
    }
}
