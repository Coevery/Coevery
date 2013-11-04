using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coevery.Common.Extensions {
    public static class CoeveryStringExtension {
        private const string PartSuffix = "Part";

        public static string ToPartName(this string source) {
            return source + PartSuffix;
        }

        public static string RemovePartSuffix(this string source) {
            return source.EndsWith(PartSuffix)
                       ? source.Substring(0, source.LastIndexOf(PartSuffix, StringComparison.CurrentCulture))
                       : source;
        }
    }
}