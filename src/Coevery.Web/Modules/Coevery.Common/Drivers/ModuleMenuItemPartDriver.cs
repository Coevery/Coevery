using System.Web.Mvc;
using Coevery.Common.Models;
using Coevery.Common.ViewModels;
using JetBrains.Annotations;
using Coevery;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;
using Coevery.Core.Navigation;
using Coevery.Core.Settings.Metadata.Records;
using Coevery.Data;
using Coevery.Localization;
using Coevery.Security;
using System.Linq;

namespace Coevery.Common.Drivers {
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