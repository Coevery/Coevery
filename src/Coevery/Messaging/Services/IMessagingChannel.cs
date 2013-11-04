using System.Collections.Generic;
using Coevery.Messaging.Models;

namespace Coevery.Messaging.Services {
    public interface IMessagingChannel : IDependency {
        /// <summary>
        /// Actually sends the message though this channel
        /// </summary>
        void SendMessage(MessageContext message);

        /// <summary>
        /// Provides all the handled services, the user can choose from when receving messages
        /// </summary>
        IEnumerable<string> GetAvailableServices();
    }
}
