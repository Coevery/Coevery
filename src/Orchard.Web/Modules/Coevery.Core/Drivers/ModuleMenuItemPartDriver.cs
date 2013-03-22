using Coevery.Core.Models;
using Coevery.Core.ViewModels;
using JetBrains.Annotations;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Navigation;

using Orchard.Localization;
using Orchard.Security;

namespace Coevery.Core.Drivers {
    [UsedImplicitly]
    public class ModuleMenuItemPartDriver : ContentPartDriver<ModuleMenuItemPart> {
        private readonly IContentManager _contentManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IWorkContextAccessor _workContextAccessor;

        public ModuleMenuItemPartDriver(
            IContentManager contentManager,
            IAuthorizationService authorizationService, 
            IWorkContextAccessor workContextAccessor) {
            _contentManager = contentManager;
            _authorizationService = authorizationService;
            _workContextAccessor = workContextAccessor;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override DriverResult Editor(ModuleMenuItemPart part, dynamic shapeHelper) {
            return ContentShape("Parts.ModuleMenuItem.Edit",
                                () => {
                                    var model = new ModuleMenuItemEditViewModel() {
                                        ContentItemId = part.Content == null ? -1 : part.Content.Id,
                                        Part = part
                                    };
                                    return shapeHelper.EditorTemplate(TemplateName: "Parts.ModuleMenuItem.Edit", Model: model, Prefix: Prefix);
                                });
        }

        protected override DriverResult Editor(ModuleMenuItemPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            var currentUser = _workContextAccessor.GetContext().CurrentUser;
            if (!_authorizationService.TryCheckAccess(Permissions.ManageMainMenu, currentUser, part))
                return null;

            var model = new ModuleMenuItemEditViewModel();

            if(updater.TryUpdateModel(model, Prefix, null, null)) {
                var contentItem = _contentManager.Get(model.ContentItemId);
                if(contentItem == null) {
                    updater.AddModelError("ContentItemId", T("You must select a Content Item"));
                }
                else {
                    part.Content = contentItem;
                }
            }

            return Editor(part, shapeHelper);
        }

        protected override void Importing(ModuleMenuItemPart part, ImportContentContext context) {
            var contentItemId = context.Attribute(part.PartDefinition.Name, "ContentItem");
            if (contentItemId != null) {
                var contentItem = context.GetItemFromSession(contentItemId);
                part.Content = contentItem;
            }
            else {
                part.Content = null;
            }
        }

        protected override void Exporting(ModuleMenuItemPart part, ExportContentContext context)
        {
            if (part.Content != null) {
                var contentItem = _contentManager.Get(part.Content.Id);
                if (contentItem != null) {
                    var containerIdentity = _contentManager.GetItemMetadata(contentItem).Identity;
                    context.Element(part.PartDefinition.Name).SetAttributeValue("ContentItem", containerIdentity.ToString());
                }
            }
        }
    }
}