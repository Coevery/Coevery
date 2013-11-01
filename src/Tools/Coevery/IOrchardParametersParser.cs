using Coevery.Parameters;

namespace Coevery {
    public interface IOrchardParametersParser {
        OrchardParameters Parse(CommandParameters parameters);
    }
}