using System.IO;
using Coevery.Host;

namespace Coevery.HostContext {
    public class CommandHostContext {
        public CommandReturnCodes StartSessionResult { get; set; }
        public CommandReturnCodes RetryResult { get; set; }

        public CoeveryParameters Arguments { get; set; }
        public DirectoryInfo CoeveryDirectory { get; set; }
        public bool DisplayUsageHelp { get; set; }
        public CommandHost CommandHost { get; set; }
        public Logger Logger { get; set; }
    }
}