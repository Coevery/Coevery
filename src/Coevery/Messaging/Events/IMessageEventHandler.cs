using Coevery.Events;
using Coevery.Messaging.Models;

namespace Coevery.Messaging.Events {
    public interface IMessageEventHandler : IEventHandler {
        void Sending(MessageContext context);
        void Sent(MessageContext context);
    }
}
