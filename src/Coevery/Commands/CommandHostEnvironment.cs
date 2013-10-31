using System.Reflection;
using Coevery.Environment;
using Coevery.Localization;

namespace Coevery.Commands {
    internal class CommandHostEnvironment : HostEnvironment {
        public CommandHostEnvironment() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public override void RestartAppDomain() {
            throw new CoeveryCommandHostRetryException(T("A change of configuration requires the session to be restarted."));
        }
    }
}