using System.Reflection.Emit;
using Coevery.Events;

namespace Coevery.Common.Events {
    public interface IDynamicTypeGenerationEvents : IEventHandler {
        void OnBuilded(ModuleBuilder moduleBuilder);
    }
}
