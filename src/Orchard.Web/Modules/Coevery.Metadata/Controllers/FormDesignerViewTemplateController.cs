using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Coevery.Metadata.HtmlControlProviders;
using Coevery.Metadata.Services;
using Coevery.Metadata.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Themes;
using System.Linq;

namespace Coevery.Metadata.Controllers
{

    public class FormDesignerViewTemplateController : Controller
    {
        private IContentDefinitionManager _contentDefinitionManager;
        private IContentDefinitionService _contentDefinitionService;
        private readonly IContentManager _contentManager;

        public FormDesignerViewTemplateController(IOrchardServices orchardServices,
            IContentDefinitionManager contentDefinitionManager,
            IContentDefinitionService contentDefinitionService,
            IContentManager contentManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _contentDefinitionService = contentDefinitionService;
            _contentManager = contentManager;
            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }


        public ActionResult Dynamic(string id, string returnUrl)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            //FormDesignerViewModel viewModel = new FormDesignerViewModel();
            //var typeDefinition = _contentDefinitionManager.GetTypeDefinition(id);
            //viewModel.TypeDefinition = typeDefinition;
            //viewModel.ContentPart = typeDefinition.Parts.FirstOrDefault(t => t.PartDefinition.Name == typeDefinition.Name);
            //viewModel.HtmlControlDescs = _contentDefinitionService.GetHtmlDescForTypeFields(viewModel.ContentPart);

            var contentItem = _contentManager.New(id);
            dynamic model = _contentManager.BuildEditor(contentItem);
            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(id);
            string layout = contentTypeDefinition.Settings.ContainsKey("Layout")
                                ? contentTypeDefinition.Settings["Layout"]
                                : null;
            model.Layout = layout;

            return View((object)model);
        }

    }
}
