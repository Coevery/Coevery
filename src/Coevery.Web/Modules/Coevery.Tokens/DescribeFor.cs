using System.Collections.Generic;
using Coevery.Localization;

namespace Coevery.Tokens {
    public abstract class DescribeFor {
        public abstract IEnumerable<TokenDescriptor> Tokens { get; }
        public abstract LocalizedString Name { get; }
        public abstract LocalizedString Description { get; }
        public abstract DescribeFor Token(string token, LocalizedString name, LocalizedString description);
        public abstract DescribeFor Token(string token, LocalizedString name, LocalizedString description, string chainTarget);
    }
}