using System;
using System.Linq;
using Coevery.Core.Services;
using Coevery.Entities.Settings;
using Coevery.Entities.ViewModels;
using Coevery.FormDesigner.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Utility.Extensions;

namespace Coevery.Entities.Services {
    public class FieldService : Component, IFieldService {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly ISchemaUpdateService _schemaUpdateService;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public FieldService(
            IContentDefinitionService contentDefinitionService,
            ISchemaUpdateService schemaUpdateService,
            IContentDefinitionManager contentDefinitionManager) {
            _contentDefinitionService = contentDefinitionService;
            _schemaUpdateService = schemaUpdateService;
            _contentDefinitionManager = contentDefinitionManager;
        }

        public void CreateCheck(string entityName, AddFieldViewModel viewModel, IUpdateModel updateModel) {
            
        }

        //public void Create(string entityName, AddFieldViewModel viewModel, IUpdateModel updateModel) {
        //    try {
        //        _contentDefinitionService.AddFieldToPart(viewModel.Name, viewModel.DisplayName, viewModel.FieldTypeName, entityName);
        //        _contentDefinitionService.CreateField(entityName, viewModel.Name, updateModel);
        //        _schemaUpdateService.CreateColumn(entityName, viewModel.Name, viewModel.FieldTypeName);
        //    }
        //    catch (Exception ex) {
        //        updateModel.AddModelError("ErrorInfo", T("Add field failed."));
        //    }
        //}

        public void Delete(string name, string parentname) {
            _contentDefinitionService.RemoveFieldFromPart(name, parentname);
            var layoutManager = new LayoutManager(_contentDefinitionManager);
            layoutManager.DeleteField(parentname, name);
            _schemaUpdateService.DropColumn(parentname, name);
        }
    }
}