using System.Collections.Generic;

namespace Coevery.Parameters {
    public interface ICommandParametersParser {
        CommandParameters Parse(IEnumerable<string> args);
    }
}