using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Coevery.ContentManagement;
using Coevery.Core.Common.Models;
using Coevery.Core.Containers.Extensions;
using Coevery.Core.Containers.Models;
using Coevery.Core.Contents;
using Coevery.Core.Feeds;
using Coevery.DisplayManagement;
using Coevery.Mvc;
using Coevery.Themes;
using Coevery.UI.Navigation;
using Coevery.Settings;
using Coevery.Localization;

namespace Coevery.Core.Containers.Controllers {

    public class ItemController : Controller {
        private readonly IContentManager _contentManager;
        private readonly ISiteService _siteService;
        private readonly IFeedManager _feedManager;

        public ItemController(
            IContentManager contentManager, 
            IShapeFactory shapeFactory,
            ISiteService siteService,
            IFeedManager feedManager, 
            ICoeveryServices services) {

            _contentManager = contentManager;
            _siteService = siteService;
            _feedManager = feedManager;
            Shape = shapeFactory;
            Services = services;
            T = NullLocalizer.Instance;
        }

        dynamic Shape { get; set; }
        public ICoeveryServices Services { get; private set; }

        public Localizer T { get; set; }
        [Themed]
        public ActionResult Display(int id, PagerParameters pagerParameters) {
            var container = _contentManager
                .Get(id, VersionOptions.Published)
                .As<ContainerPart>();

            if (container == null) {
                return HttpNotFound(T("Container not found").Text);
            }

            if (!Services.Authorizer.Authorize(Permissions.ViewContent, container, T("Cannot view content"))) {
                return new HttpUnauthorizedResult();
            }

            // TODO: (PH) Find a way to apply PagerParameters via a driver so we can lose this controller
            container.PagerParameters = pagerParameters;
            var model = _contentManager.BuildDisplay(container);

            return new ShapeResult(this, model);
        }

    }
}