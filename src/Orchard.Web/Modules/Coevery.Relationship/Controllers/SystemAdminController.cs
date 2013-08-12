using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using Coevery.Core.Services;
using Coevery.Relationship.Records;
using Coevery.Relationship.Services;
using Coevery.Relationship.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Notify;
using Orchard.Core.Contents.Controllers;

namespace Coevery.Relationship.Controllers {
    public class SystemAdminController : Controller, IUpdateModel {
        private readonly IRelationshipService _relationshipService;

        public SystemAdminController(IOrchardServices orchardServices,
            IRelationshipService relationshipService) {
            Services = orchardServices;
            _relationshipService = relationshipService;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; private set; }
        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        [HttpPost]
        public ActionResult FieldNames(string entityName, int version) {
            var optionsHtml = new StringBuilder();
            foreach (var option in _relationshipService.GetFieldNames(entityName)) {
                optionsHtml.Append("<option value='"+option.Value);
                if (option.Selected) {
                    optionsHtml.Append("' selected = 'selected");
                }
                optionsHtml.Append("'>" + option.Text + "</option>");
            }
            return Json(new {
                result = optionsHtml.ToString(),
                version = version
            });
        }

        public ActionResult EditOneToMany(string id) {
            return View(new RelationshipViewModel<OneToManyRelationshipModel> {
                EntityList = _relationshipService.EntityNames,
                RelationshipRecord = new OneToManyRelationshipModel()
            });
        }

        [HttpPost]
        public ActionResult EditOneToMany(string entityName, OneToManyRelationshipModel oneToMany) {
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult EditManyToMany(string id) {
            return View(new RelationshipViewModel<ManyToManyRelationshipModel> {
                EntityList = _relationshipService.EntityNames,
                RelationshipRecord = new ManyToManyRelationshipModel()
            });
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return base.TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}