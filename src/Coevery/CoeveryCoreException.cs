using System;
using System.Runtime.Serialization;
using Coevery.Localization;

namespace Coevery {
    [Serializable]
    public class CoeveryCoreException : Exception {
        private readonly LocalizedString _localizedMessage;

        public CoeveryCoreException(LocalizedString message)
            : base(message.Text) {
            _localizedMessage = message;
        }

        public CoeveryCoreException(LocalizedString message, Exception innerException)
            : base(message.Text, innerException) {
            _localizedMessage = message;
        }

        protected CoeveryCoreException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        public LocalizedString LocalizedMessage { get { return _localizedMessage; } }
    }
}