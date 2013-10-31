using JetBrains.Annotations;
using Coevery.Core.Settings.Models;
using Coevery.Data;
using Coevery.ContentManagement.Handlers;

namespace Coevery.Core.Settings.Handlers {
    [UsedImplicitly]
    public class SiteSettingsPartHandler : ContentHandler {
        public SiteSettingsPartHandler(IRepository<SiteSettingsPartRecord> repository, IRepository<SiteSettings2PartRecord> repository2) {
            Filters.Add(new ActivatingFilter<SiteSettingsPart>("Site"));
            Filters.Add(new ActivatingFilter<SiteSettings2Part>("Site"));
            Filters.Add(StorageFilter.For(repository));
            Filters.Add(StorageFilter.For(repository2));
        }
    }
}