using System.Collections.Generic;
using Coevery.Localization;

namespace Coevery.Tokens {
    public abstract class DescribeContext {
        public abstract IEnumerable<TokenTypeDescriptor> Describe(params string[] targets);
        public abstract DescribeFor For(string target);
        public abstract DescribeFor For(string target, LocalizedString name, LocalizedString description);
    }
}