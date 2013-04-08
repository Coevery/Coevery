using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Dynamic.Settings;
using Coevery.Metadata.Services;
using Coevery.Metadata.ViewModels;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Utility.Extensions;

namespace Coevery.Metadata.Controllers {
    public class MetadataController : ApiController {
        private readonly IContentDefinitionService _contentDefinitionService;

        public MetadataController(IContentDefinitionService contentDefinitionService) {
            _contentDefinitionService = contentDefinitionService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        // GET api/metadata/metadata
        public IEnumerable<object> Get() {
            var metadataTypes = _contentDefinitionService.GetTypes();

            var query = from type in metadataTypes
                        let setting = type.Settings.GetModel<DynamicTypeSettings>()
                        select new {type.DisplayName, type.Name, setting.IsDeployed};
            return query;
        }

        // GET api/metadata/metadata
        public object Get(string name) {
            var metadataTypes = _contentDefinitionService.GetTypes();

            var query = from type in metadataTypes
                        let setting = type.Settings.GetModel<DynamicTypeSettings>()
                        let fields = type.Fields.Select(f => new {f.DisplayName, Name = f.FieldDefinition.Name.CamelFriendly()})
                        where type.Name == name
                        select new {type.DisplayName, type.Name, setting.IsDeployed, Fields = fields};
            return query.SingleOrDefault();
        }

        //// GET api/metadata/metadata/name
        //public virtual EditTypeViewDto Get(string name) {
        //    var metadataType = _contentDefinitionService.GetType(name);
        //    var setting = metadataType.Settings.GetModel<DynamicTypeSettings>();
        //    var metadata = new EditTypeViewDto() {
        //        DisplayName = metadataType.DisplayName,
        //        Name = metadataType.Name,
        //        IsEnabled = setting.IsEnabled,
        //        IsDeployed = setting.IsDeployed
        //    };
        //    metadata.Fields = new List<FieldViewModelDto>();

        //    if (metadataType.Fields != null)
        //        metadataType.Fields.ToList().ForEach(c => {
        //            metadata.Fields.Add(new FieldViewModelDto() {DisplayName = c.DisplayName, Name = c.Name, FieldTypeDisplayName = c.FieldDefinition.Name});
        //        });

        //    return metadata;
        //}

        //// PUT api/metadata/metadata/name
        //public virtual HttpResponseMessage Put(string id, EditTypeViewDto editTypeModel) {

        //    var typeViewModel = _contentDefinitionService.GetType(id);
        //    typeViewModel.DisplayName = editTypeModel.DisplayName;
        //    typeViewModel.Settings["DynamicTypeSettings.IsEnabled"] = editTypeModel.IsEnabled.ToString();
        //    typeViewModel.Settings["DynamicTypeSettings.IsDeployed"] = editTypeModel.IsDeployed.ToString();
        //    if (String.IsNullOrWhiteSpace(typeViewModel.DisplayName)) {
        //        ModelState.AddModelError("DisplayName", T("The Content Type name can't be empty.").ToString());
        //    }

        //    if (_contentDefinitionService.GetTypes().Any(t => String.Equals(t.DisplayName.Trim(), typeViewModel.DisplayName.Trim(), StringComparison.OrdinalIgnoreCase)
        //                                                      && !String.Equals(t.Name, id))) {
        //        ModelState.AddModelError("DisplayName", T("A type with the same name already exists.").ToString());
        //    }
        //    if (!ModelState.IsValid) {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }

        //    _contentDefinitionService.AlterType(typeViewModel, this);
        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        //// POST api/metadata/metadata
        //public virtual HttpResponseMessage Post(EditTypeViewDto editTypeModel) {

        //    editTypeModel.DisplayName = editTypeModel.DisplayName ?? String.Empty;
        //    editTypeModel.Name = editTypeModel.Name ?? String.Empty;
        //    if (String.IsNullOrWhiteSpace(editTypeModel.DisplayName)) {
        //        ModelState.AddModelError("DisplayName", T("The Display Name name can't be empty.").ToString());
        //    }

        //    if (String.IsNullOrWhiteSpace(editTypeModel.Name)) {
        //        ModelState.AddModelError("Name", T("The Content Type Id can't be empty.").ToString());
        //    }

        //    if (_contentDefinitionService.GetTypes().Any(t => String.Equals(t.Name.Trim(), editTypeModel.Name.Trim(), StringComparison.OrdinalIgnoreCase))) {
        //        ModelState.AddModelError("Name", T("A type with the same Id already exists.").ToString());
        //    }

        //    if (!String.IsNullOrWhiteSpace(editTypeModel.Name) && !editTypeModel.Name[0].IsLetter()) {
        //        ModelState.AddModelError("Name", T("The technical name must start with a letter.").ToString());
        //    }

        //    if (_contentDefinitionService.GetTypes().Any(t => String.Equals(t.DisplayName.Trim(), editTypeModel.DisplayName.Trim(), StringComparison.OrdinalIgnoreCase))) {
        //        ModelState.AddModelError("DisplayName", T("A type with the same Display Name already exists.").ToString());
        //    }

        //    if (!ModelState.IsValid) {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }
        //    _contentDefinitionService.AddType(editTypeModel);
        //    _contentDefinitionService.AddPartToType("CommonPart", editTypeModel.Name);
        //    return Request.CreateResponse(HttpStatusCode.Created, editTypeModel);
        //}

        //// DELETE api/metadata/metadata/name
        //public virtual HttpResponseMessage Delete(string name) {
        //    var typeViewModel = _contentDefinitionService.GetType(name);
        //    if (typeViewModel == null) {
        //        ModelState.AddModelError("Name", T("A type with the name not exists.").ToString());
        //    }
        //    if (!ModelState.IsValid) {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }
        //    _contentDefinitionService.RemoveType(name, true);
        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        //bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {

        //    return true;
        //}

        //void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
        //    ModelState.AddModelError(key, errorMessage.ToString());
        //}

    }
}