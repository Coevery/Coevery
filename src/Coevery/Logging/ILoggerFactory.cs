using System;

namespace Coevery.Logging {
    public interface ILoggerFactory {
        ILogger CreateLogger(Type type);
    }
}