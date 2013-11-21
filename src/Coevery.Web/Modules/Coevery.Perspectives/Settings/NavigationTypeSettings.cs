using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.Navigation.ViewModels;

namespace Coevery.Perspectives.Settings {
    public enum NavigationType {
        EntityNavigation = 0,
        CustomItemNavigation = 1,
        CustomLinkNavigation = 2
    }

    public class NavigationTypeSettings {
        public MenuItemEntry MenuItem{get; set; }
        public PositionTreeModel TreeInfo { get; set; }
        public int PerspectiveId { get; set; }
        public NavigationType MenuItemType { get; set; }
    }
}