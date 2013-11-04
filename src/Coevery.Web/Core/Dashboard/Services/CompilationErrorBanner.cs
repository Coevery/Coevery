using System.Linq;
using System.Collections.Generic;
using Coevery.Environment.Extensions;
using Coevery.Localization;
using Coevery.UI.Admin.Notification;
using Coevery.UI.Notify;

namespace Coevery.Core.Dashboard.Services {
    public class CompilationErrorBanner : INotificationProvider {
        private readonly ICriticalErrorProvider _errorProvider;

        public CompilationErrorBanner(ICriticalErrorProvider errorProvider) {
            _errorProvider = errorProvider;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public IEnumerable<NotifyEntry> GetNotifications() {
            return _errorProvider.GetErrors()
                .Select(message => new NotifyEntry { Message = message, Type = NotifyType.Error });
        }
    }
}
