using System.Collections.Generic;

namespace Coevery.Commands {
    public interface ICommandManager : IDependency {
        void Execute(CommandParameters parameters);
        IEnumerable<CommandDescriptor> GetCommandDescriptors();
    }
}