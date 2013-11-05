using Coevery.Parameters;

namespace Coevery {
    public interface ICoeveryParametersParser {
        CoeveryParameters Parse(CommandParameters parameters);
    }
}