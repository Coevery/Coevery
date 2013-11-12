using Coevery.UI.Navigation;
using Coevery.ContentManagement;

namespace Coevery.Core.Navigation.ViewModels {
    public class MenuItemEntry {
        public int MenuItemId { get; set; }
        public bool IsMenuItem { get; set; }
        
        public string Text { get; set; }
        public string Url { get; set; }
        public string Position { get; set; }
        public string Description { get; set; }

        public ContentItem ContentItem { get; set; }
    }
}