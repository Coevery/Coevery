using System.Linq;
using System.Collections.Generic;
using Coevery.ContentManagement;
using Coevery.Localization;
using Coevery.Messaging.Services;
using Coevery.UI.Admin.Notification;
using Coevery.UI.Notify;
using Coevery.Users.Models;

namespace Coevery.Users.Services {
    public class MissingSettingsBanner : INotificationProvider {
        private readonly ICoeveryServices _coeveryServices;
        private readonly IMessageManager _messageManager;

        public MissingSettingsBanner(ICoeveryServices coeveryServices, IMessageManager messageManager) {
            _coeveryServices = coeveryServices;
            _messageManager = messageManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public IEnumerable<NotifyEntry> GetNotifications() {

            var registrationSettings = _coeveryServices.WorkContext.CurrentSite.As<RegistrationSettingsPart>();

            if ( registrationSettings != null &&
                    ( registrationSettings.UsersMustValidateEmail ||
                    registrationSettings.NotifyModeration ||
                    registrationSettings.EnableLostPassword ) &&
                !_messageManager.GetAvailableChannelServices().Contains("email") ) {
                yield return new NotifyEntry { Message = T("Some Coevery.User settings require an Email channel to be enabled."), Type = NotifyType.Warning };
            }
        }
    }
}
