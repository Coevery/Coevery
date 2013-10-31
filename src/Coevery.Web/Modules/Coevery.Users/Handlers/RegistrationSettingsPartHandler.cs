using JetBrains.Annotations;
using Coevery.ContentManagement;
using Coevery.Data;
using Coevery.ContentManagement.Handlers;
using Coevery.Localization;
using Coevery.Users.Models;

namespace Coevery.Users.Handlers {
    [UsedImplicitly]
    public class RegistrationSettingsPartHandler : ContentHandler {
        public RegistrationSettingsPartHandler(IRepository<RegistrationSettingsPartRecord> repository) {
            T = NullLocalizer.Instance;
            Filters.Add(new ActivatingFilter<RegistrationSettingsPart>("Site"));
            Filters.Add(StorageFilter.For(repository));
            Filters.Add(new TemplateFilterForRecord<RegistrationSettingsPartRecord>("RegistrationSettings", "Parts/Users.RegistrationSettings", "users"));
        }

        public Localizer T { get; set; }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            if (context.ContentItem.ContentType != "Site")
                return;
            base.GetItemMetadata(context);
            context.Metadata.EditorGroupInfo.Add(new GroupInfo(T("Users")));
        }
    }
}