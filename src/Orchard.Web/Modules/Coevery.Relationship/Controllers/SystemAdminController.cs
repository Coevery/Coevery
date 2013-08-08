using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Coevery.Core.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using Orchard.Core.Contents.Controllers;

namespace Coevery.Relationship.Controllers {
    public class SystemAdminController : Controller, IUpdateModel {
        //private readonly IContentDefinitionService _contentDefinitionService;

        public SystemAdminController(IOrchardServices orchardServices) {
            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult EditOneToMany(string id) {
            return View();
        }

        public ActionResult EditManyToMany(string id) {
            return View();
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return base.TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}