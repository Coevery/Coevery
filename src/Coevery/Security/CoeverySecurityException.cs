using System;
using System.Runtime.Serialization;
using Coevery.ContentManagement;
using Coevery.Localization;

namespace Coevery.Security {
    [Serializable]
    public class CoeverySecurityException : CoeveryCoreException {
        public CoeverySecurityException(LocalizedString message) : base(message) { }
        public CoeverySecurityException(LocalizedString message, Exception innerException) : base(message, innerException) { }
        protected CoeverySecurityException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public string PermissionName { get; set; }
        public IUser User { get; set; }
        public IContent Content { get; set; }
    }
}
