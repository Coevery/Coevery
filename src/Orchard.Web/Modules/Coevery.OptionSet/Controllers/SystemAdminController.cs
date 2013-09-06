using System.Web.Mvc;
using Coevery.Core.Services;
using Orchard;
using Orchard.ContentManagement.MetaData;
using Orchard.Localization;
using Orchard.Logging;
using IContentDefinitionEditorEvents = Orchard.ContentManagement.MetaData.IContentDefinitionEditorEvents;
using IContentDefinitionService = Coevery.Entities.Services.IContentDefinitionService;

namespace Coevery.OptionSet.Controllers {
    public class SystemAdminController : Controller {
        private readonly IContentDefinitionService _contentDefinitionService;
        private readonly ISchemaUpdateService _schemaUpdateService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentDefinitionEditorEvents _contentDefinitionEditorEvents;

        public SystemAdminController(IOrchardServices orchardServices
            ,IContentDefinitionService contentDefinitionService
            , ISchemaUpdateService schemaUpdateService
            ,IContentDefinitionManager contentDefinitionManager
            ,IContentDefinitionEditorEvents contentDefinitionEditorEvents) {
            Services = orchardServices;
            _contentDefinitionService = contentDefinitionService;
            _schemaUpdateService = schemaUpdateService;
            T = NullLocalizer.Instance;
            _contentDefinitionManager = contentDefinitionManager;
            _contentDefinitionEditorEvents = contentDefinitionEditorEvents;
        }

        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult List(string returnUrl) {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}