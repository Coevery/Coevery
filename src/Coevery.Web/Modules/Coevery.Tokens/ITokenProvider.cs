using Coevery.Events;

namespace Coevery.Tokens {
    public interface ITokenProvider : IEventHandler {
        void Describe(DescribeContext context);
        void Evaluate(EvaluateContext context);
    }
}