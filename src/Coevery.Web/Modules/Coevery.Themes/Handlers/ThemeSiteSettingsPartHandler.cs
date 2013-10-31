using JetBrains.Annotations;
using Coevery.Data;
using Coevery.ContentManagement.Handlers;
using Coevery.Themes.Models;

namespace Coevery.Themes.Handlers {
    [UsedImplicitly]
    public class ThemeSiteSettingsPartHandler : ContentHandler {
        public ThemeSiteSettingsPartHandler(IRepository<ThemeSiteSettingsPartRecord> repository) {
            Filters.Add(new ActivatingFilter<ThemeSiteSettingsPart>("Site"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}