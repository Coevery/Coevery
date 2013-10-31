using System.Collections.Generic;
using System.Linq;
using Coevery.Localization;
using Coevery.Scripting;
using Coevery.Widgets.Services;

namespace Coevery.Widgets.RuleEngine {
    public class RuleManager : IRuleManager {
        private readonly IEnumerable<IRuleProvider> _ruleProviders;
        private readonly IEnumerable<IScriptExpressionEvaluator> _evaluators;

        public RuleManager(IEnumerable<IRuleProvider> ruleProviders, IEnumerable<IScriptExpressionEvaluator> evaluators) {
            _ruleProviders = ruleProviders;
            _evaluators = evaluators;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public bool Matches(string expression) {
            var evaluator = _evaluators.FirstOrDefault();
            if (evaluator == null) {
                throw new CoeveryException(T("There are currently not scripting engine enabled"));
            }

            var result = evaluator.Evaluate(expression, new List<IGlobalMethodProvider> { new GlobalMethodProvider(this) });
            if (!(result is bool)) {
                throw new CoeveryException(T("Expression is not a boolean value"));
            }
            return (bool)result;
        }

        private class GlobalMethodProvider : IGlobalMethodProvider {
            private readonly RuleManager _ruleManager;

            public GlobalMethodProvider(RuleManager ruleManager) {
                _ruleManager = ruleManager;
            }

            public void Process(GlobalMethodContext context) {
                var ruleContext = new RuleContext {
                    FunctionName = context.FunctionName,
                    Arguments = context.Arguments.ToArray(),
                    Result = context.Result
                };

                foreach(var ruleProvider in _ruleManager._ruleProviders) {
                    ruleProvider.Process(ruleContext);
                }

                context.Result = ruleContext.Result;
            }
        }
    }
}