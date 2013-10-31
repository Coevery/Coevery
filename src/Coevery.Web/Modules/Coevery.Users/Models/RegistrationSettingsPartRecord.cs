using System.ComponentModel.DataAnnotations;
using Coevery.ContentManagement.Records;

namespace Coevery.Users.Models {
    public class RegistrationSettingsPartRecord : ContentPartRecord {
        public virtual bool UsersCanRegister { get; set; }
        public virtual bool UsersMustValidateEmail { get; set; }
        [StringLength(255)]
        public virtual string ValidateEmailRegisteredWebsite { get; set; }
        [StringLength(255)]
        public virtual string ValidateEmailContactEMail { get; set; }

        public virtual bool UsersAreModerated { get; set; }
        public virtual bool NotifyModeration { get; set; }
        public virtual string NotificationsRecipients { get; set; }

        public virtual bool EnableLostPassword { get; set; }
    }
}