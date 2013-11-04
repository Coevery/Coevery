using System;
using Autofac;

namespace Coevery.Environment {
    public interface IShellContainerRegistrations {
        Action<ContainerBuilder> Registrations { get; }
    }

    public class ShellContainerRegistrations : IShellContainerRegistrations {
        public ShellContainerRegistrations() {
            Registrations = builder => { return; };
        }

        public Action<ContainerBuilder> Registrations { get; private set; }
    }
}
