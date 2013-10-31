using System.Collections.Generic;

namespace Coevery.Commands {
    public class CommandHandlerDescriptor {
        public IEnumerable<CommandDescriptor> Commands { get; set; }
    }
}
