using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Orchard.ContentManagement;

namespace Coevery.Leads.Controllers
{
    public class ViewTemplateController : Controller
    {
        private readonly IContentManager _contentManager;

        public ViewTemplateController(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Detail()
        {
            var id = "Lead";
            var contentItem = _contentManager.New(id);

            dynamic model = _contentManager.BuildEditor(contentItem);

            return View((object)model);
        }
    }
}
