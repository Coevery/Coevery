using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Coevery.Metadata.Services;
using Coevery.Metadata.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Data;
using Orchard.Localization;
using Orchard.WebApi.Common;
using Orchard.Utility.Extensions;

namespace Coevery.Metadata.Controllers
{

    public class FieldController : ApiController
    {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        public FieldController(IContentDefinitionService contentDefinitionService, 
            IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionService = contentDefinitionService;
            _contentDefinitionManager = contentDefinitionManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        
        // GET api/metadata/field/name
        public virtual FieldViewModelDto Get(string name,string parentname)
        {
            var partViewModel = _contentDefinitionService.GetPart(parentname);
            var fieldViewModel = partViewModel.Fields.FirstOrDefault(x => x.Name == name);
            if (fieldViewModel == null)
                return new FieldViewModelDto();
            return new FieldViewModelDto() { DisplayName = fieldViewModel.DisplayName, Name = fieldViewModel.Name, ParentName = parentname };
        }

        // PUT api/metadata/field/name
        public virtual HttpResponseMessage Put(string id, FieldViewModelDto fieldModel)
        {
            var partViewModel = _contentDefinitionService.GetPart(fieldModel.ParentName);


            // prevent null reference exception in validation
            fieldModel.DisplayName = fieldModel.DisplayName ?? String.Empty;

            // remove extra spaces
            fieldModel.DisplayName = fieldModel.DisplayName.Trim();

            if (String.IsNullOrWhiteSpace(fieldModel.DisplayName))
            {
                ModelState.AddModelError("DisplayName", T("The Display Name name can't be empty.").ToString());
            }

            if (_contentDefinitionService.GetPart(partViewModel.Name).Fields.Any(t => t.Name != fieldModel.Name && String.Equals(t.DisplayName.Trim(), fieldModel.DisplayName.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                ModelState.AddModelError("DisplayName", T("A field with the same Display Name already exists.").ToString());
            }

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var field = _contentDefinitionManager.GetPartDefinition(fieldModel.ParentName).Fields.FirstOrDefault(x => x.Name == id);

            if (field == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            field.DisplayName = fieldModel.DisplayName;
            _contentDefinitionManager.StorePartDefinition(partViewModel._Definition);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // POST api/metadata/field
        public virtual HttpResponseMessage Post(FieldViewModelDto fieldModel)
        {
            var partViewModel = _contentDefinitionService.GetPart(fieldModel.ParentName);
            var typeViewModel = _contentDefinitionService.GetType(fieldModel.ParentName);
            if (partViewModel == null) {
                // id passed in might be that of a type w/ no implicit field
                if (typeViewModel != null) {
                    partViewModel = new EditPartViewModel {Name = typeViewModel.Name};
                    _contentDefinitionService.AddPart(new CreatePartViewModel {Name = partViewModel.Name});
                    _contentDefinitionService.AddPartToType(partViewModel.Name, typeViewModel.Name);
                }
            }

            fieldModel.DisplayName = fieldModel.DisplayName ?? String.Empty;
            fieldModel.DisplayName = fieldModel.DisplayName.Trim();
            fieldModel.Name = fieldModel.Name ?? String.Empty;

            if (String.IsNullOrWhiteSpace(fieldModel.DisplayName)) {
                ModelState.AddModelError("DisplayName", T("The Display Name name can't be empty.").ToString());
            }

            if (String.IsNullOrWhiteSpace(fieldModel.Name)) {
                ModelState.AddModelError("Name", T("The Technical Name can't be empty.").ToString());
            }

            if (_contentDefinitionService.GetPart(partViewModel.Name).Fields.Any(t => String.Equals(t.Name.Trim(), fieldModel.Name.Trim(), StringComparison.OrdinalIgnoreCase))) {
                ModelState.AddModelError("Name", T("A field with the same name already exists.").ToString());
            }

            if (!String.IsNullOrWhiteSpace(fieldModel.Name) && !fieldModel.Name[0].IsLetter()) {
                ModelState.AddModelError("Name", T("The technical name must start with a letter.").ToString());
            }

            if (!String.Equals(fieldModel.Name, fieldModel.Name.ToSafeName(), StringComparison.OrdinalIgnoreCase)) {
                ModelState.AddModelError("Name", T("The technical name contains invalid characters.").ToString());
            }

            if (_contentDefinitionService.GetPart(partViewModel.Name).Fields.Any(t => String.Equals(t.DisplayName.Trim(), Convert.ToString(fieldModel.DisplayName).Trim(), StringComparison.OrdinalIgnoreCase))) {
                ModelState.AddModelError("DisplayName", T("A field with the same Display Name already exists.").ToString());
            }

            if (!ModelState.IsValid) {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            _contentDefinitionService.AddFieldToPart(fieldModel.Name, fieldModel.DisplayName, fieldModel.FieldTypeName, partViewModel.Name);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE api/metadata/field/name
        public virtual HttpResponseMessage Delete(string name,string parentname)
        {
            _contentDefinitionService.RemoveFieldFromPart(name, parentname);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}