using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.Localization;
using Coevery.UI.Admin.Notification;
using Coevery.UI.Notify;

namespace Coevery.Data.Migration {
    public class DataMigrationNotificationProvider: INotificationProvider {
        private readonly IDataMigrationManager _dataMigrationManager;

        public DataMigrationNotificationProvider(IDataMigrationManager dataMigrationManager) {
            _dataMigrationManager = dataMigrationManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public IEnumerable<NotifyEntry> GetNotifications() {
            var features = _dataMigrationManager.GetFeaturesThatNeedUpdate();

            if(features.Any()) {
                yield return new NotifyEntry { Message = T("Some features need to be upgraded: {0}", String.Join(", ", features)), Type = NotifyType.Warning};
            }
        }
    }
}
