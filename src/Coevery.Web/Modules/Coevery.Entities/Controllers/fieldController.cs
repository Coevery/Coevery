using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Entities.Services;
using Coevery.Entities.Settings;
using Coevery.Localization;
using Coevery.Utility.Extensions;

namespace Coevery.Entities.Controllers {
    public class FieldController : ApiController {
        private readonly IContentMetadataService _contentMetadataService;
        private readonly ISettingService _settingService;

        public FieldController(
            ISettingService settingService,
            IContentMetadataService contentMetadataService) {
            _settingService = settingService;
            _contentMetadataService = contentMetadataService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        // GET api/metadata/field
        public object Get(int name, int page, int rows) {
            var metadataTypes = _contentMetadataService.GetFieldsList(name);

            var query = from field in metadataTypes
                        let fieldType = field.ContentFieldDefinitionRecord.Name
                        let setting = _settingService.ParseSetting(field.Settings)
                        select new {
                            field.Name,
                            field.Id,
                            DisplayName = setting["DisplayName"],
                            FieldType = fieldType.CamelFriendly(),
                            Type = setting.GetModel<FieldSettings>(fieldType + "Settings").IsSystemField
                                ? "System Field" : "User Field",
                            ControllField = string.Empty
                        };

            var totalRecords = query.Count();
            return new {
                total = Convert.ToInt32(Math.Ceiling((double)totalRecords / rows)),
                page = page,
                records = totalRecords,
                rows = query
            };
        }

        // DELETE api/metadata/field/name
        public virtual HttpResponseMessage Delete([FromUri]string[] name, string entityName)
        {
            var state = true;
            foreach (var n in name) {
                var stateService = _contentMetadataService.DeleteField(n, entityName);
                state = state && stateService;

            }
            return state
                ? Request.CreateResponse(HttpStatusCode.OK)
                : Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid id!");
        }
    }
}