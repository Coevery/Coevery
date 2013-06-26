using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Entities.Services;
using Coevery.Entities.Settings;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Utility.Extensions;

namespace Coevery.Entities.Controllers {
    public class EntityController : ApiController {
        private readonly IContentDefinitionService _contentDefinitionService;

        public EntityController(IContentDefinitionService contentDefinitionService) {
            _contentDefinitionService = contentDefinitionService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        // GET api/Entities/Entity
        public IEnumerable<object> Get() {
            var metadataTypes = _contentDefinitionService.GetUserDefinedTypes();

            var query = from type in metadataTypes
                        let setting = type.Settings.GetModel<DynamicTypeSettings>()
                        select new { type.DisplayName, type.Name, setting.IsDeployed };
            return query;
        }

        // GET api/Entities/Entity/:entityName
        public object Get(string name) {
            var metadataTypes = _contentDefinitionService.GetUserDefinedTypes().Where(c => c.Name == name);

            var query = from type in metadataTypes
                        let setting = type.Settings.GetModel<DynamicTypeSettings>()
                        let fields = type.Fields.Select(f => new { f.Name, f.DisplayName, FieldType = f.FieldDefinition.Name.CamelFriendly(), IsSystemField = bool.Parse(f.Settings[f.FieldDefinition.Name + "Settings.IsSystemField"]) })
                        select new { type.DisplayName, type.Name, setting.IsDeployed, Fields = fields };
            var entityType = query.SingleOrDefault();
            return entityType;
        }
    }
}