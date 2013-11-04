using System;
using System.Runtime.Serialization;
using Coevery.Localization;

namespace Coevery.Commands {
    [Serializable]
    public class CoeveryCommandHostRetryException : CoeveryCoreException {
        public CoeveryCommandHostRetryException(LocalizedString message)
            : base(message) {
        }

        public CoeveryCommandHostRetryException(LocalizedString message, Exception innerException)
            : base(message, innerException) {
        }

        protected CoeveryCommandHostRetryException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }
    }
}