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
using Orchard.Core.Contents;
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
            if (string.IsNullOrWhiteSpace(entityName) || entityName == "0") {
                return Json(new {
                    result = "<option value=''>  </option>",
                    version = version
                });
            }

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

        public ActionResult Relationships() {
            return View();
        }

        public ActionResult CreateOneToMany(string id) {
            if (!Services.Authorizer.Authorize(Permissions.PublishContent, T("Not allowed to edit a content.")))
                return new HttpUnauthorizedResult();

            return View(new OneToManyRelationshipModel {
                EntityList = _relationshipService.GetEntityNames()
            });
        }

        [HttpPost]
        public ActionResult CreateOneToMany(OneToManyRelationshipModel oneToMany) {
            if (!Services.Authorizer.Authorize(Permissions.PublishContent, T("Not allowed to edit a content.")))
                return new HttpUnauthorizedResult();

            if (!_relationshipService.CreateRelationship(oneToMany)) {
                ModelState.AddModelError("OneToManyRelation",T("Create relationship failed.").ToString());
                return HttpNotFound();
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult CreateManyToMany(string id) {
            if (!Services.Authorizer.Authorize(Permissions.PublishContent, T("Not allowed to edit a content.")))
                return new HttpUnauthorizedResult();

            return View(new ManyToManyRelationshipModel {
                EntityList = _relationshipService.GetEntityNames()
            });
        }

        [HttpPost]
        public ActionResult CreateManyToMany(ManyToManyRelationshipModel manyToMany) {
            if (!Services.Authorizer.Authorize(Permissions.PublishContent, T("Not allowed to edit a content.")))
                return new HttpUnauthorizedResult();

            if (!_relationshipService.CreateRelationship(manyToMany)) {
                ModelState.AddModelError("ManyToManyRelation", T("Create relationship failed.").ToString());
                return HttpNotFound();
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return base.TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}