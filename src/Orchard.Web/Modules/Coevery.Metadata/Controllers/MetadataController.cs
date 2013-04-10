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
            var metadataTypes = _contentDefinitionService.GetUserDefinedTypes();

            var query = from type in metadataTypes
                        let setting = type.Settings.GetModel<DynamicTypeSettings>()
                        select new {type.DisplayName, type.Name, setting.IsDeployed};
            return query;
        }

        // GET api/metadata/metadata
        public object Get(string name) {
            var metadataTypes = _contentDefinitionService.GetUserDefinedTypes();

            var query = from type in metadataTypes
                        let setting = type.Settings.GetModel<DynamicTypeSettings>()
                        let fields = type.Fields.Select(f => new {f.Name, f.DisplayName, FieldType = f.FieldDefinition.Name.CamelFriendly()})
                        where type.Name == name
                        select new {type.DisplayName, type.Name, setting.IsDeployed, Fields = fields};
            return query.SingleOrDefault();
        }
    }
}