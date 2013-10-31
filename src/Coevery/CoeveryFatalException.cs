using System;
using System.Runtime.Serialization;
using Coevery.Localization;

namespace Coevery {
    [Serializable]
    public class CoeveryFatalException : Exception {
        private readonly LocalizedString _localizedMessage;

        public CoeveryFatalException(LocalizedString message)
            : base(message.Text) {
            _localizedMessage = message;
        }

        public CoeveryFatalException(LocalizedString message, Exception innerException)
            : base(message.Text, innerException) {
            _localizedMessage = message;
        }

        protected CoeveryFatalException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        public LocalizedString LocalizedMessage { get { return _localizedMessage; } }
    }
}
