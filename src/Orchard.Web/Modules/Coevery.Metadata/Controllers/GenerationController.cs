using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.Metadata.DynamicTypeGeneration;
using Coevery.Metadata.Services;
using Orchard.Localization;

namespace Coevery.Metadata.Controllers
{
    public class GenerationController : ApiController
    {
        private readonly IDynamicAssemblyBuilder _dynamicAssemblyBuilder;

        public GenerationController(
            IDynamicAssemblyBuilder dynamicAssemblyBuilder)
        {
            _dynamicAssemblyBuilder = dynamicAssemblyBuilder;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public HttpResponseMessage Post([FromBody]string name)
        {
            try {
                bool successful = _dynamicAssemblyBuilder.Build();

                if (successful)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            }
            catch (Exception) {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            }
        }
    }
}