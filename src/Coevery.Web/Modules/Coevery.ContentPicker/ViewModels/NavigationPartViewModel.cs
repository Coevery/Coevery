using System.Collections.Generic;
using Coevery.ContentManagement;
using Coevery.ContentPicker.Models;
using Coevery.Core.Navigation.Models;

namespace Coevery.ContentPicker.ViewModels {
    public class NavigationPartViewModel {
        public IEnumerable<MenuPart> ContentMenuItems { get; set; }
        public NavigationPart Part { get; set; }
        public IEnumerable<ContentItem> Menus { get; set; }
        public string MenuText { get; set; }
        public bool AddMenuItem { get; set; }
        public int CurrentMenuId { get; set; }
    }
}