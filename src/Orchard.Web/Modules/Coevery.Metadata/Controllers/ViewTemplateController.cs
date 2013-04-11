using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Coevery.Metadata.Extensions;
using Coevery.Metadata.Services;
using Coevery.Metadata.Settings;
using Coevery.Metadata.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents.Controllers;
using Orchard.Core.Contents.Settings;
using Orchard.Environment.Configuration;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI;
using Orchard.UI.Notify;
using Orchard.Utility.Extensions;
using Orchard.Themes;

namespace Coevery.Metadata.Controllers
{
    public class ViewTemplateController : Controller, IUpdateModel
    {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IPlacementService _placementService;
        private readonly Lazy<IEnumerable<IShellSettingsManagerEventHandler>> _settingsManagerEventHandlers;
        private readonly ShellSettings _settings;

        public ViewTemplateController(
            IOrchardServices orchardServices,
            IContentDefinitionService contentDefinitionService,
            IContentDefinitionManager contentDefinitionManager,
            IPlacementService placementService,
            Lazy<IEnumerable<IShellSettingsManagerEventHandler>> settingsManagerEventHandlers,
            ShellSettings settings
            )
        {
            Services = orchardServices;
            _contentDefinitionService = contentDefinitionService;
            _contentDefinitionManager = contentDefinitionManager;
            _placementService = placementService;
            _settingsManagerEventHandlers = settingsManagerEventHandlers;
            _settings = settings;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        #region Types

        public ActionResult List()
        {
            return View();
        }


        public ActionResult Create()
        {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to create a content type.")))
                return new HttpUnauthorizedResult();

            var typeViewModel = _contentDefinitionService.GetType(string.Empty);

            return View(typeViewModel);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST(CreateTypeViewModel viewModel)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to create a content type.")))
                return new HttpUnauthorizedResult();

            viewModel.DisplayName = viewModel.DisplayName ?? String.Empty;
            viewModel.Name = viewModel.Name ?? String.Empty;

            if (String.IsNullOrWhiteSpace(viewModel.DisplayName))
            {
                ModelState.AddModelError("DisplayName", T("The Display Name name can't be empty.").ToString());
            }

            if (String.IsNullOrWhiteSpace(viewModel.Name))
            {
                ModelState.AddModelError("Name", T("The Content Type Id can't be empty.").ToString());
            }

            if (_contentDefinitionService.GetTypes().Any(t => String.Equals(t.Name.Trim(), viewModel.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("Name", T("A type with the same Id already exists.").ToString());
            }

            if (!String.IsNullOrWhiteSpace(viewModel.Name) && !viewModel.Name[0].IsLetter())
            {
                ModelState.AddModelError("Name", T("The technical name must start with a letter.").ToString());
            }

            if (_contentDefinitionService.GetTypes().Any(t => String.Equals(t.DisplayName.Trim(), viewModel.DisplayName.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("DisplayName", T("A type with the same Display Name already exists.").ToString());
            }

            if (!ModelState.IsValid)
            {
                Services.TransactionManager.Cancel();
                return View(viewModel);
            }

            var contentTypeDefinition = _contentDefinitionService.AddType(viewModel.Name, viewModel.DisplayName);

            // adds CommonPart by default
            _contentDefinitionService.AddPartToType("CommonPart", viewModel.Name);

            var typeViewModel = new EditTypeViewModel(contentTypeDefinition);


            Services.Notifier.Information(T("The \"{0}\" content type has been created.", typeViewModel.DisplayName));

            return RedirectToAction("AddPartsTo", new { id = typeViewModel.Name });
        }

        public ActionResult ContentTypeName(string displayName, int version)
        {
            return Json(new
            {
                result = _contentDefinitionService.GenerateContentTypeNameFromDisplayName(displayName),
                version = version
            });
        }

        public ActionResult FieldName(string partName, string displayName, int version)
        {
            return Json(new
            {
                result = _contentDefinitionService.GenerateFieldNameFromDisplayName(partName, displayName),
                version = version
            });
        }

        public ActionResult Edit(string id)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content type.")))
                return new HttpUnauthorizedResult();

            var typeViewModel = _contentDefinitionService.GetType(id);

            if (typeViewModel == null)
                return HttpNotFound();

            return View(typeViewModel);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Delete")]
        public ActionResult Delete(string id)
        {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to delete a content type.")))
                return new HttpUnauthorizedResult();

            var typeViewModel = _contentDefinitionService.GetType(id);

            if (typeViewModel == null)
                return HttpNotFound();

            _contentDefinitionService.RemoveType(id, true);

            Services.Notifier.Information(T("\"{0}\" has been removed.", typeViewModel.DisplayName));

            return RedirectToAction("List");
        }

        
        #endregion

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties)
        {
            return base.TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage)
        {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}
