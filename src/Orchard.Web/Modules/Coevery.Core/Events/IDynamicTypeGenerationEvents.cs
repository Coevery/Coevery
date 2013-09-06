using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Orchard.Events;

namespace Coevery.Core.Events {
    public interface IDynamicTypeGenerationEvents : IEventHandler {
        void OnBuilded(ModuleBuilder moduleBuilder);
    }
}
