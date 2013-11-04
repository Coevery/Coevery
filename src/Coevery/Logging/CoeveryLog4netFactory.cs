using System;
using System.Configuration;
using Castle.Core.Logging;
using log4net;
using log4net.Config;
using Coevery.Environment;

namespace Coevery.Logging {
    public class CoeveryLog4netFactory : AbstractLoggerFactory {
        private static bool _isFileWatched = false;

        public CoeveryLog4netFactory(IHostEnvironment hostEnvironment) 
            : this(ConfigurationManager.AppSettings["log4net.Config"], hostEnvironment) { }

        public CoeveryLog4netFactory(string configFilename, IHostEnvironment hostEnvironment) {
            if (!_isFileWatched && !string.IsNullOrWhiteSpace(configFilename) && hostEnvironment.IsFullTrust) {
                // Only monitor configuration file in full trust
                XmlConfigurator.ConfigureAndWatch(GetConfigFile(configFilename));
                _isFileWatched = true;
            }
        }

        public override Castle.Core.Logging.ILogger Create(string name, LoggerLevel level) {
            throw new NotSupportedException("Logger levels cannot be set at runtime. Please review your configuration file.");
        }

        public override Castle.Core.Logging.ILogger Create(string name) {
            return new CoeveryLog4netLogger(LogManager.GetLogger(name), this);
        }
    }
}
