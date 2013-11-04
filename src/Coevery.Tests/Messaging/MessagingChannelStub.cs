using System.Collections.Generic;
using Coevery.Messaging.Services;
using Coevery.Messaging.Models;

namespace Coevery.Tests.Messaging {
    public class MessagingChannelStub : IMessagingChannel {
        public List<MessageContext> Messages { get; private set; }

        public MessagingChannelStub() {
            Messages = new List<MessageContext>();
        }

        #region IMessagingChannel Members

        public void SendMessage(MessageContext message) {
            Messages.Add(message);
        }

        public IEnumerable<string> GetAvailableServices() {
            yield return "email";
        }

        #endregion
    }
}
