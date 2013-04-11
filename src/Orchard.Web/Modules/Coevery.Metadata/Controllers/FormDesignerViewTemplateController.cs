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
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public FormDesignerViewTemplateController(
            IOrchardServices orchardServices,
            IContentManager contentManager, IContentDefinitionManager contentDefinitionManager)
        {
            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        [Themed]
        public ActionResult Index(string id, string returnUrl)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var contentItem = _contentManager.New(id);
            dynamic model = _contentManager.BuildEditor(contentItem);
            var contentTypeDefinition = contentItem.TypeDefinition;
            string layout = contentTypeDefinition.Settings.ContainsKey("Layout")
                                ? contentTypeDefinition.Settings["Layout"]
                                : null;

            var viewModel = Services.New.ViewModel().Content(model);
            viewModel.Layout = layout;
            viewModel.Fields = _contentDefinitionManager.GetPartDefinition(id).Fields;
            viewModel.DisplayName = contentItem.TypeDefinition.DisplayName;

            return View((object)viewModel);
        }

    }
}
