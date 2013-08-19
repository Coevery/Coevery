using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using JetBrains.Annotations;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.Settings;
using Coevery.Taxonomies.Models;
using Coevery.Taxonomies.ViewModels;
using Coevery.Taxonomies.Services;
using Orchard.UI.Notify;

namespace Coevery.Taxonomies.Controllers {

    [ValidateInput(false)]
    public class AdminController : Controller, IUpdateModel {
        private readonly ITaxonomyService _taxonomyService;

        public AdminController(
            IOrchardServices services,
            ITaxonomyService taxonomyService) {
            Services = services;
            _taxonomyService = taxonomyService;
            T = NullLocalizer.Instance;
        }

        public IOrchardServices Services { get; set; }
        protected virtual ISite CurrentSite { get; [UsedImplicitly] private set; }

        public Localizer T { get; set; }

        public ActionResult Index() {
            var taxonomies = _taxonomyService.GetTaxonomies();
            var entries = taxonomies.Select(CreateTaxonomyEntry).ToList();
            var model = new TaxonomyAdminIndexViewModel { Taxonomies = entries };
            return View(model);
        }

        [HttpPost]
        public ActionResult Index(FormCollection input) {
            var viewModel = new TaxonomyAdminIndexViewModel { Taxonomies = new List<TaxonomyEntry>(), BulkAction = new TaxonomiesAdminIndexBulkAction() };

            if (!TryUpdateModel(viewModel)) {
                return View(viewModel);
            }

            var checkedEntries = viewModel.Taxonomies.Where(t => t.IsChecked);
            switch (viewModel.BulkAction) {
                case TaxonomiesAdminIndexBulkAction.None:
                    break;
                case TaxonomiesAdminIndexBulkAction.Delete:
                    if (!Services.Authorizer.Authorize(Permissions.ManageTaxonomies, T("Couldn't delete taxonomy")))
                        return new HttpUnauthorizedResult();

                    foreach (var entry in checkedEntries) {
                        _taxonomyService.DeleteTaxonomy(_taxonomyService.GetTaxonomy(entry.Id));
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Create() {
            return View();
        }

        public ActionResult Delete(int id) {
            if (!Services.Authorizer.Authorize(Permissions.CreateTaxonomy, T("Couldn't delete taxonomy")))
                return new HttpUnauthorizedResult();

            var taxonomy = _taxonomyService.GetTaxonomy(id);
            if (taxonomy == null) {
                return HttpNotFound();
            }

            _taxonomyService.DeleteTaxonomy(taxonomy);

            return RedirectToAction("Index");
        }        

        private static TaxonomyEntry CreateTaxonomyEntry(TaxonomyPart taxonomy) {
            return new TaxonomyEntry {
                Id = taxonomy.Id,
                Name = taxonomy.Name,
                IsInternal = taxonomy.IsInternal,
                ContentItem = taxonomy.ContentItem,
                IsChecked = false,
            };
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }
    }
}
