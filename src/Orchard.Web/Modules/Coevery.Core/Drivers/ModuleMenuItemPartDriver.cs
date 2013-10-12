using System.Web.Mvc;
using Coevery.Core.Models;
using Coevery.Core.ViewModels;
using JetBrains.Annotations;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.Core.Navigation;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Security;
using System.Linq;

namespace Coevery.Core.Drivers {
    [UsedImplicitly]
    public class ModuleMenuItemPartDriver : ContentPartDriver<ModuleMenuItemPart> {
        private readonly IAuthorizationService _authorizationService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IRepository<ContentTypeDefinitionRecord> _contentTypeRepository;

        public ModuleMenuItemPartDriver(
            IContentManager contentManager,
            IAuthorizationService authorizationService,
            IWorkContextAccessor workContextAccessor,
            IRepository<ContentTypeDefinitionRecord> contentTypeRepository) {
            _authorizationService = authorizationService;
            _workContextAccessor = workContextAccessor;
            _contentTypeRepository = contentTypeRepository;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override DriverResult Editor(ModuleMenuItemPart part, dynamic shapeHelper) {
            var contentTypes = _contentTypeRepository.Fetch(t => !t.Hidden).ToList();
            var selectLists = contentTypes.Select(t => new SelectListItem {
                Selected = part.Record.ContentTypeDefinitionRecord != null && part.Record.ContentTypeDefinitionRecord.Id.Equals(t.Id),
                Text = t.Name,
                Value = t.Id.ToString()
            });
            return ContentShape("Parts_ModuleMenuItem_Edit",
                () => {
                    var model = new ModuleMenuItemEditViewModel() {
                        ContenTypeId = part.Record.ContentTypeDefinitionRecord == null ? -1 : part.Record.ContentTypeDefinitionRecord.Id,
                        Part = part,
                        ContentTypes = selectLists
                    };
                    return shapeHelper.EditorTemplate(TemplateName: "Parts.ModuleMenuItem.Edit", Model: model, Prefix: Prefix);
                });
        }

        protected override DriverResult Editor(ModuleMenuItemPart part, IUpdateModel updater, dynamic shapeHelper) {
            var currentUser = _workContextAccessor.GetContext().CurrentUser;
            if (!_authorizationService.TryCheckAccess(Permissions.ManageMainMenu, currentUser, part)) {
                return null;
            }

            var model = new ModuleMenuItemEditViewModel();

            if (updater.TryUpdateModel(model, Prefix, null, null)) {
                var contentTypeRecord = _contentTypeRepository.Get(model.ContenTypeId);
                if (contentTypeRecord == null) {
                    updater.AddModelError("ContentTypeId", T("You must select a ContentType Item"));
                }
                else {
                    part.Record.ContentTypeDefinitionRecord = contentTypeRecord;
                }
            }

            return Editor(part, shapeHelper);
        }
    }
}