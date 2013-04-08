using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Coevery.Metadata.ViewModels;
using Orchard;
using Orchard.ContentManagement.MetaData;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using System.Linq;

namespace Coevery.Metadata.Controllers {

    public class HomeController : Controller
    {
        private IContentDefinitionManager _contentDefinitionManager;
       
        public HomeController(
            IOrchardServices orchardServices,
            IContentDefinitionManager contentDefinitionManager
            )
        {
            _contentDefinitionManager = contentDefinitionManager;
            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }


        [Themed]
        public ActionResult Dynamic(string id,string returnUrl)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            FormDesignerViewModel viewModel = new FormDesignerViewModel();
            var typeDefinition = _contentDefinitionManager.GetTypeDefinition(id);
            viewModel.TypeDefinition = typeDefinition;
            viewModel.ContentPart = typeDefinition.Parts.FirstOrDefault(t => t.PartDefinition.Name == typeDefinition.Name);

            return View(viewModel);
        }

    }
}
