﻿using System;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Navigation.Models;
using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;
using Orchard.Utility;

namespace Orchard.Core.Navigation.Drivers {
    [UsedImplicitly]
    public class MenuPartDriver : ContentPartDriver<MenuPart> {
        private readonly IAuthorizationService _authorizationService;
        private readonly INavigationManager _navigationManager;

        public MenuPartDriver(IAuthorizationService authorizationService, INavigationManager navigationManager) {
            _authorizationService = authorizationService;
            _navigationManager = navigationManager;
            T = NullLocalizer.Instance;
        }

        public virtual IUser CurrentUser { get; set; }
        public Localizer T { get; set; }

        protected override DriverResult Editor(MenuPart part, dynamic shapeHelper) {
            if (!_authorizationService.TryCheckAccess(Permissions.ManageMainMenu, CurrentUser, part))
                return null;

            // TODO: andrerod convert to new shape API. Location code kept for reference.
            return ContentPartTemplate(part, "Parts/Navigation.EditMenuPart"); //.Location(part.GetLocation("Editor"));
        }

        protected override DriverResult Editor(MenuPart part, IUpdateModel updater, dynamic shapeHelper) {
            if (!_authorizationService.TryCheckAccess(Permissions.ManageMainMenu, CurrentUser, part)) {
                return null;
            }

            if (string.IsNullOrEmpty(part.MenuPosition)) {
                part.MenuPosition = Position.GetNext(_navigationManager.BuildMenu("main"));
            }

            updater.TryUpdateModel(part, Prefix, null, null);

            if (part.OnMainMenu && string.IsNullOrEmpty(part.MenuText)) {
                updater.AddModelError("MenuText", T("The MenuText field is required"));
            }

            // TODO: andrerod convert to new shape API. Location code kept for reference.
            return ContentPartTemplate(part, "Parts/Navigation.EditMenuPart"); //.Location(part.GetLocation("Editor"));
        }
    }
}