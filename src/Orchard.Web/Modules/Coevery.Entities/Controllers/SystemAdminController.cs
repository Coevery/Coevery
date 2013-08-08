using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Data.Entity.Design.PluralizationServices;
using System.Web.Mvc;
using Coevery.Core.Services;
using Coevery.Fields.Services;
using Coevery.Fields.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Utility.Extensions;
using Orchard.UI.Notify;
using Orchard.Core.Contents.Controllers;
using EditTypeViewModel = Coevery.Entities.ViewModels.EditTypeViewModel;
using IContentDefinitionService = Coevery.Entities.Services.IContentDefinitionService;

namespace Coevery.Entities.Controllers {
    public class SystemAdminController : Controller, IUpdateModel {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly ISchemaUpdateService _schemaUpdateService;
        private readonly IFieldService _fieldService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        public SystemAdminController(IOrchardServices orchardServices
            ,IContentDefinitionService contentDefinitionService
            , ISchemaUpdateService schemaUpdateService
            ,IFieldService fieldService
            ,IContentDefinitionManager contentDefinitionManager
            ) {
            Services = orchardServices;
            _contentDefinitionService = contentDefinitionService;
            _schemaUpdateService = schemaUpdateService;
            T = NullLocalizer.Instance;
            _fieldService = fieldService;
            _contentDefinitionManager = contentDefinitionManager;
        }

        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult List(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult Create() {
            //if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to create a content type.")))
            //    return new HttpUnauthorizedResult();

            var typeViewModel = _contentDefinitionService.GetType(string.Empty);

            return View(typeViewModel);
        }

        public ActionResult EntityName(string displayName, int version) {
            return Json(new {
                result = _contentDefinitionService.GenerateContentTypeNameFromDisplayName(displayName),
                version = version
            });
        }

        public ActionResult FieldName(string displayName, int version)
        {
            return Json(new
            {
                result = _contentDefinitionService.GenerateContentTypeNameFromDisplayName(displayName),
                version = version
            });
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST(EditTypeViewModel viewModel) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to create a content type.")))
                return new HttpUnauthorizedResult();

            viewModel.DisplayName = viewModel.DisplayName ?? String.Empty;
            viewModel.Name = (viewModel.Name ?? viewModel.DisplayName).ToSafeName();

            viewModel.FieldLabel = viewModel.FieldLabel ?? String.Empty;
            viewModel.FieldName = (viewModel.FieldName ?? viewModel.FieldLabel).ToSafeName();

            if (String.IsNullOrWhiteSpace(viewModel.DisplayName)) {
                ModelState.AddModelError("DisplayName", T("The Display Name name can't be empty.").ToString());
            }

            if (String.IsNullOrWhiteSpace(viewModel.Name)) {
                ModelState.AddModelError("Name", T("The Content Type Id can't be empty.").ToString());
            }

            if (String.IsNullOrWhiteSpace(viewModel.FieldLabel))
            {
                ModelState.AddModelError("DisplayName", T("The Field Label name can't be empty.").ToString());
            }

            if (String.IsNullOrWhiteSpace(viewModel.FieldName))
            {
                ModelState.AddModelError("Name", T("The Field Name can't be empty.").ToString());
            }

            if (!PluralizationService.CreateService(new CultureInfo("en-US")).IsSingular(viewModel.Name)) {
                ModelState.AddModelError("Name", T("The name should be singular.").ToString());
            }

            if (_contentDefinitionService.GetTypes().Any(t => String.Equals(t.Name.Trim(), viewModel.Name.Trim(), StringComparison.OrdinalIgnoreCase))) {
                ModelState.AddModelError("Name", T("A type with the same Id already exists.").ToString());
            }

            if (!String.IsNullOrWhiteSpace(viewModel.Name) && !viewModel.Name[0].IsLetter()) {
                ModelState.AddModelError("Name", T("The technical name must start with a letter.").ToString());
            }

            if (_contentDefinitionService.GetTypes().Any(t => String.Equals(t.DisplayName.Trim(), viewModel.DisplayName.Trim(), StringComparison.OrdinalIgnoreCase))) {
                ModelState.AddModelError("DisplayName", T("A type with the same Display Name already exists.").ToString());
            }

            if (!PluralizationService.CreateService(new CultureInfo("en-US")).IsSingular(viewModel.FieldName))
            {
                ModelState.AddModelError("FieldName", T("The field name should be singular.").ToString());
            }

            if (!String.IsNullOrWhiteSpace(viewModel.FieldName) && !viewModel.FieldName[0].IsLetter())
            {
                ModelState.AddModelError("FieldName", T("The technical field name must start with a letter.").ToString());
            }

            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var temp = (from values in ModelState
                            from error in values.Value.Errors
                             select error.ErrorMessage).ToArray();              
                return Content(string.Concat(temp));
            }

            _contentDefinitionService.AlterType(viewModel, this);

            _contentDefinitionService.AddPartToType(viewModel.Name, viewModel.Name);

            //var contentTypeDefinition = _contentDefinitionService.AddType(viewModel.DisplayName,viewModel.Name);

            // adds CommonPart by default
            //_contentDefinitionService.AddPartToType("CommonPart", viewModel.Name);

            //var typeViewModel = new EditTypeViewModel(contentTypeDefinition);


            Services.Notifier.Information(T("The \"{0}\" content type has been created.", viewModel.DisplayName));
            _schemaUpdateService.CreateTable(viewModel.Name.Trim());

            AddFieldViewModel addViewModel = new AddFieldViewModel();
            addViewModel.DisplayName = viewModel.FieldLabel.Trim();
            addViewModel.Name = viewModel.FieldName.Trim();
            addViewModel.FieldTypeName = "CoeveryTextField";
            _fieldService.Create(viewModel.Name.Trim(), addViewModel, this);
            var part = _contentDefinitionManager.GetPartDefinition(viewModel.Name.Trim());
            var field = part.Fields.FirstOrDefault(x => x.Name == viewModel.FieldName);
            if (field != null)
            {
                field.Settings["CoeveryTextFieldSettings.IsDispalyField"] = "true";
                field.Settings["CoeveryTextFieldSettings.Required"] = "true";
                field.Settings["CoeveryTextFieldSettings.ReadOnly"] = "true";
                field.Settings["CoeveryTextFieldSettings.AlwaysInLayout"] = "true";
                field.Settings["CoeveryTextFieldSettings.IsSystemField"] = "true";
            }
            _contentDefinitionManager.StorePartDefinition(part);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Edit(string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content type.")))
                return new HttpUnauthorizedResult();

            var typeViewModel = _contentDefinitionService.GetType(id);

            if (typeViewModel == null)
                return HttpNotFound();

            return View(typeViewModel);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("submit.Delete")]
        public ActionResult Delete(string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to delete a content type.")))
                return new HttpUnauthorizedResult();

            var typeViewModel = _contentDefinitionService.GetType(id);

            if (typeViewModel == null)
                return HttpNotFound();

            _contentDefinitionService.RemoveType(id, true);

            Services.Notifier.Information(T("\"{0}\" has been removed.", typeViewModel.DisplayName));

            return RedirectToAction("List");
        }

        public ActionResult Detail(string id) {
            if (!Services.Authorizer.Authorize(Permissions.EditContentTypes, T("Not allowed to edit a content type.")))
                return new HttpUnauthorizedResult();

            var typeViewModel = _contentDefinitionService.GetType(id);

            if (typeViewModel == null)
                return HttpNotFound();

            return View(typeViewModel);
        }

        public ActionResult Fields() {
            return View();
        }

        public ActionResult Relationships() {
            return View();
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return base.TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}