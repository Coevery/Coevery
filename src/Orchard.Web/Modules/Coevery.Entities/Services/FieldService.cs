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
        private readonly ILayoutManager _layoutManager;

        public FieldService(
            IContentDefinitionService contentDefinitionService,
            ISchemaUpdateService schemaUpdateService,
            IContentDefinitionManager contentDefinitionManager,
            ILayoutManager layoutManager) {
            _contentDefinitionService = contentDefinitionService;
            _schemaUpdateService = schemaUpdateService;
            _contentDefinitionManager = contentDefinitionManager;
            _layoutManager = layoutManager;
        }

        public void Delete(string name, string parentname) {
            _contentDefinitionService.RemoveFieldFromPart(name, parentname);
            _layoutManager.DeleteField(parentname, name);
            _schemaUpdateService.DropColumn(parentname, name);
        }
    }
}