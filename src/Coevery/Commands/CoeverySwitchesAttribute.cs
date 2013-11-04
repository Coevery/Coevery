using System;
using System.Collections.Generic;
using System.Linq;

namespace Coevery.Commands {
    [AttributeUsage(AttributeTargets.Method)]
    public class CoeverySwitchesAttribute : Attribute {
        private readonly string _switches;

        public CoeverySwitchesAttribute(string switches) {
            _switches = switches;
        }

        public IEnumerable<string> Switches {
            get {
                return (_switches ?? "").Trim().Split(',').Select(s => s.Trim());
            }
        }
    }
}
