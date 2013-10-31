using System.Collections.Generic;

namespace Coevery.Scripting {
    public interface IScriptExpressionEvaluator : ISingletonDependency {
        object Evaluate(string expression, IEnumerable<IGlobalMethodProvider> providers);
    }
}