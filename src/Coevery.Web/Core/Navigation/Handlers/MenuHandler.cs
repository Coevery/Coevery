using JetBrains.Annotations;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Handlers;
using Coevery.Core.Navigation.Services;
using Coevery.Core.Title.Models;

namespace Coevery.Core.Navigation.Handlers {
    [UsedImplicitly]
    public class MenuHandler : ContentHandler {
        private readonly IMenuService _menuService;
        private readonly IContentManager _contentManager;

        public MenuHandler(IMenuService menuService, IContentManager contentManager) {
            _menuService = menuService;
            _contentManager = contentManager;
        }

        protected override void Removing(RemoveContentContext context) {
            if (context.ContentItem.ContentType != "Menu") {
                return;
            }

            // remove all menu items
            var menuParts = _menuService.GetMenuParts(context.ContentItem.Id);

            foreach(var menuPart in menuParts) {
                _contentManager.Remove(menuPart.ContentItem);
            }
        }
    }
}