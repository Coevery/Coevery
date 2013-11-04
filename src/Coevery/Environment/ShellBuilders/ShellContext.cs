using Autofac;
using Coevery.Environment.Configuration;
using Coevery.Environment.Descriptor.Models;
using Coevery.Environment.ShellBuilders.Models;

namespace Coevery.Environment.ShellBuilders {
    public class ShellContext {
        public ShellSettings Settings { get; set; }
        public ShellDescriptor Descriptor { get; set; }
        public ShellBlueprint Blueprint { get; set; }
        public ILifetimeScope LifetimeScope { get; set; }
        public ICoeveryShell Shell { get; set; }
    }
}