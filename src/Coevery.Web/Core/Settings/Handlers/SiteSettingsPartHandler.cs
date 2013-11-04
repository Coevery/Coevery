using JetBrains.Annotations;
using Coevery.Core.Settings.Models;
using Coevery.Data;
using Coevery.ContentManagement.Handlers;

namespace Coevery.Core.Settings.Handlers {
    [UsedImplicitly]
    public class SiteSettingsPartHandler : ContentHandler {
        public SiteSettingsPartHandler() {
            Filters.Add(new ActivatingFilter<SiteSettingsPart>("Site"));
        }
    }
}