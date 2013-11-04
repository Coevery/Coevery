using System.Collections.Generic;
using Coevery.UI.Notify;

namespace Coevery.UI.Admin.Notification {
    public interface INotificationManager : IDependency {
        /// <summary>
        /// Returns all notifications to display per zone
        /// </summary>
        IEnumerable<NotifyEntry> GetNotifications();
    }
}
