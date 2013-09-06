using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Core.Services;
using Coevery.Entities.Events;
using Coevery.Entities.Services;
using Coevery.Entities.Settings;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Utility.Extensions;

namespace Coevery.Entities.Controllers {
    public class EntityController : ApiController {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly ISchemaUpdateService _schemaUpdateService;
        private readonly IEntityEvents _entityEvents;

        public EntityController(
            IContentDefinitionService contentDefinitionService,
            ISchemaUpdateService schemaUpdateService,
            IEntityEvents entityEvents) {
            _contentDefinitionService = contentDefinitionService;
            _schemaUpdateService = schemaUpdateService;
            _entityEvents = entityEvents;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        // GET api/Entities/Entity
        public IEnumerable<object> Get() {
            var metadataTypes = _contentDefinitionService.GetUserDefinedTypes();

            var query = from type in metadataTypes
                let setting = type.Settings.GetModel<DynamicTypeSettings>()
                select new {type.DisplayName, type.Name, setting.IsDeployed};
            return query;
        }

        // GET api/Entities/Entity/:entityName
        public object Get(string name) {
            var metadataTypes = _contentDefinitionService.GetUserDefinedTypes().Where(c => c.Name == name);

            var query = from type in metadataTypes
                let setting = type.Settings.GetModel<DynamicTypeSettings>()
                let fields = type.Fields.Select(f => new {
                    f.Name,
                    f.DisplayName,
                    FieldType = f.FieldDefinition.Name.CamelFriendly(),
                    f.Settings.GetModel<FieldSettings>(f.FieldDefinition.Name + "Settings").IsSystemField
                })
                select new {type.DisplayName, type.Name, setting.IsDeployed, Fields = fields};
            var entityType = query.SingleOrDefault();
            return entityType;
        }

        // DELETE api/Entities/Entity/:entityName
        public virtual HttpResponseMessage Delete(string name) {
            var typeViewModel = _contentDefinitionService.GetType(name);

            if (typeViewModel == null) {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            }
            _entityEvents.OnDeleting(name);
            _contentDefinitionService.RemoveType(name, true);
            _schemaUpdateService.DropTable(name);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}