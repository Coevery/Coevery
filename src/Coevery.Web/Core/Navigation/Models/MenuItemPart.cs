using Coevery.ContentManagement;

namespace Coevery.Core.Navigation.Models {
    public class MenuItemPart : ContentPart<MenuItemPartRecord> {
        
        public string Url {
            get { return Record.Url; }
            set { Record.Url = value; }
        }
    }
}
