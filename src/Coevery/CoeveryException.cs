using System;
using System.Runtime.Serialization;
using Coevery.Localization;

namespace Coevery {
    [Serializable]
    public class CoeveryException : ApplicationException {
        private readonly LocalizedString _localizedMessage;

        public CoeveryException(LocalizedString message)
            : base(message.Text) {
            _localizedMessage = message;
        }

        public CoeveryException(LocalizedString message, Exception innerException)
            : base(message.Text, innerException) {
            _localizedMessage = message;
        }

        protected CoeveryException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        public LocalizedString LocalizedMessage { get { return _localizedMessage; } }
    }
}