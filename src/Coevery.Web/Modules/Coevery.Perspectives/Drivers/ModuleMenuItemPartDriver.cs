using System.Globalization;
using System.Web.Mvc;
using Coevery.Common.Models;
using Coevery.Common.ViewModels;
using Coevery.Perspectives.Services;
using JetBrains.Annotations;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;
using Coevery.Core.Navigation;
using Coevery.Core.Settings.Metadata.Records;
using Coevery.Data;
using Coevery.Localization;
using Coevery.Security;
using System.Linq;

namespace Coevery.Perspectives.Drivers {
    [UsedImplicitly]
    public class ModuleMenuItemPartDriver : ContentPartDriver<ModuleMenuItemPart> {
        private readonly IAuthorizationService _authorizationService;
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly IRepository<ContentTypeDefinitionRecord> _contentTypeRepository;
        private readonly IContentDefinitionService _contentDefinitionService;

        public ModuleMenuItemPartDriver(
            IContentManager contentManager,
            IAuthorizationService authorizationService,
            IWorkContextAccessor workContextAccessor,
            IRepository<ContentTypeDefinitionRecord> contentTypeRepository, 
            IContentDefinitionService contentDefinitionService) {
            _authorizationService = authorizationService;
            _workContextAccessor = workContextAccessor;
            _contentTypeRepository = contentTypeRepository;
            _contentDefinitionService = contentDefinitionService;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        protected override string Prefix {
            get {
                return "ModuleMenuItemPart";
            }
        }

        protected override DriverResult Editor(ModuleMenuItemPart part, dynamic shapeHelper) {
            var metadataTypes = _contentDefinitionService.GetUserDefinedTypes();
            var selectLists = metadataTypes.Select(t => new SelectListItem {
                Selected = part.ContentTypeDefinitionRecord != null && part.ContentTypeDefinitionRecord.Name == t.Name,
                Text = t.DisplayName,
                Value = t.Name
            });
            return ContentShape("Parts_ModuleMenuItem_Edit",
                () => {
                    var model = new ModuleMenuItemEditViewModel() {
                        EntityName = part.ContentTypeDefinitionRecord == null ? null : part.ContentTypeDefinitionRecord.Name,
                        IconClass = part.IconClass,
                        Entities = selectLists
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
                var contentTypeRecord = _contentTypeRepository.Table.FirstOrDefault(t => t.Name == model.EntityName);
                if (contentTypeRecord == null) {
                    updater.AddModelError("ContentTypeId", T("You must select a ContentType Item"));
                }
                if (string.IsNullOrEmpty(model.IconClass)) {
                    updater.AddModelError("IconClass", T("Icon is required."));
                }
                else {
                    part.ContentTypeDefinitionRecord = contentTypeRecord;
                    part.IconClass = model.IconClass;
                }
            }

            return Editor(part, shapeHelper);
        }
    }
}