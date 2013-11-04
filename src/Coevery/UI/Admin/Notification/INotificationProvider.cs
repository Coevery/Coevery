using System.Collections.Generic;
using Coevery.UI.Notify;

namespace Coevery.UI.Admin.Notification {
    public interface INotificationProvider : IDependency {
        /// <summary>
        /// Returns all notifications to display per zone
        /// </summary>
        IEnumerable<NotifyEntry> GetNotifications();
    }
}
