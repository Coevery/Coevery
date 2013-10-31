using Coevery.ContentManagement.Records;

namespace Coevery.Themes.Models {
    public class ThemeSiteSettingsPartRecord : ContentPartRecord {
        public virtual string CurrentThemeName { get; set; }
    }
}