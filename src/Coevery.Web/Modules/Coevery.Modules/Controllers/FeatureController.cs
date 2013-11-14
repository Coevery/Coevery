using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using Coevery.Localization;
using Coevery.Modules.Models;
using Coevery.Modules.Services;
using Newtonsoft.Json;

namespace Coevery.Modules.Controllers {
    public class FeatureController : ApiController {

        private readonly IFeatureServices _featureServices; 
        public FeatureController(IFeatureServices featureServices) {
            _featureServices = featureServices;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        //QUERY api/Modules/Feature
        public HttpResponseMessage Get() {
            var featureentrylist = _featureServices.GeFeatureCategories(FeaturesBulkAction.None,null);
            var json = JsonConvert.SerializeObject(featureentrylist, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
        }
    }
}