using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Coevery.Metadata.Services;
using Orchard.ContentManagement.MetaData;
using Orchard.Localization;

namespace Coevery.Metadata.Controllers
{
    public class GenerationController : ApiController
    {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public GenerationController(IContentDefinitionService contentDefinitionService,
            IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionService = contentDefinitionService;
            _contentDefinitionManager = contentDefinitionManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
     
        public virtual HttpResponseMessage Get(string name)
        {

            var typeViewModel = _contentDefinitionService.GetType(name);
            if (typeViewModel == null)
            {
                ModelState.AddModelError("Name", T("A type with the name not exists.").ToString());
            }
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            try
            {
                bool suc = _contentDefinitionService.GenerateType(name);

                if (suc)
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.ExpectationFailed);
                }
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            }


        }
    }
}