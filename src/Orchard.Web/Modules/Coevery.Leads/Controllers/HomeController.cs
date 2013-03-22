using System.Collections.Generic;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.DisplayManagement;
using Orchard.Themes;

namespace Coevery.Leads.Controllers
{

    public class HomeController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly dynamic _shapeFactory;

        public HomeController(IContentManager contentManager,
            IShapeFactory shapeFactory)
        {
            _contentManager = contentManager;
            _shapeFactory = shapeFactory;
        }

        [Themed]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        public ActionResult Detail(string returnUrl)
        {
            var id = "Lead";
            var contentItem = _contentManager.New(id);

            dynamic model = _contentManager.BuildEditor(contentItem);

            return View((object)model);
        }

        public ActionResult List(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [Themed]
        public ActionResult Dynamic(string returnUrl)
        {
            //var form = _shapeFactory.Create("Parts_FormDesigner_Form");
            //var tabs = new List<dynamic> {
            //    _shapeFactory.Create("Parts_FormDesigner_Tab"), 
            //    _shapeFactory.Create("Parts_FormDesigner_Tab")
            //};
            //form.Tabs = tabs;

            //var rows1 = new List<dynamic> {
            //    _shapeFactory.Create("Parts_FormDesigner_Row"), 
            //    _shapeFactory.Create("Parts_FormDesigner_Row")                
            //};
            //var rows2 = new List<dynamic> {
            //    _shapeFactory.Create("Parts_FormDesigner_Row")
            //};
            //tabs[0].Rows = rows1;
            //tabs[1].Rows = rows2;

            //var cells1 = new List<dynamic> {
            //    _shapeFactory.Create("Parts_FormDesigner_Cell")
            //};
            //var cells2 = new List<dynamic> {
            //    _shapeFactory.Create("Parts_FormDesigner_Cell"),
            //    _shapeFactory.Create("Parts_FormDesigner_Cell")
            //};
            //var cells3 = new List<dynamic> {
            //    _shapeFactory.Create("Parts_FormDesigner_Cell"),
            //    _shapeFactory.Create("Parts_FormDesigner_Cell"),
            //    _shapeFactory.Create("Parts_FormDesigner_Cell")
            //};
            //cells1[0].Width = 12;
            //rows1[0].Cells = cells1;
            //cells2[0].Width = 6;
            //cells2[1].Width = 6;
            //rows1[1].Cells = cells2;
            //cells3[0].Width = 4;
            //cells3[1].Width = 4;
            //cells3[2].Width = 4;
            //rows2[0].Cells = cells3;

            //cells1[0].Control = _shapeFactory.Create("Parts_FormDesigner_Control");
            //cells1[0].Control.Type = "text";
            //cells2[0].Control = _shapeFactory.Create("Parts_FormDesigner_Control");
            //cells2[0].Control.Type = "checkbox";
            //cells2[1].Control = _shapeFactory.Create("Parts_FormDesigner_Control");
            //cells2[1].Control.Type = "radio";
            //cells3[0].Control = _shapeFactory.Create("Parts_FormDesigner_Control");
            //cells3[0].Control.Type = "text";
            //cells3[1].Control = _shapeFactory.Create("Parts_FormDesigner_Control");
            //cells3[1].Control.Type = "textarea";
            //cells3[2].Control = _shapeFactory.Create("Parts_FormDesigner_Control");
            //cells3[2].Control.Type = "dropdown";
            //return View((object)form);

            return View();
        }
    }
}