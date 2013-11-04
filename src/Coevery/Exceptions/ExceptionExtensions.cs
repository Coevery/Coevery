using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coevery.Security;
using System.Threading;
using System.Security;
using System.Runtime.InteropServices;

namespace Coevery.Exceptions {
    public static class ExceptionExtensions {
        public static bool IsFatal(this Exception ex) {
            return ex is CoeverySecurityException ||
                ex is StackOverflowException ||
                ex is OutOfMemoryException ||
                ex is AccessViolationException ||
                ex is AppDomainUnloadedException ||
                ex is ThreadAbortException ||
                ex is SecurityException ||
                ex is SEHException;
        }
    }
}
