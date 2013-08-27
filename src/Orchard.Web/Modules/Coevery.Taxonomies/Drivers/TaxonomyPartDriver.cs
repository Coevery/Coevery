using Coevery.Taxonomies.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.Drivers;
using Coevery.Taxonomies.Models;
using Orchard.Localization;

namespace Coevery.Taxonomies.Drivers {
    public class TaxonomyPartDriver : ContentPartDriver<TaxonomyPart> {
        private readonly ITaxonomyService _taxonomyService;

        public TaxonomyPartDriver(ITaxonomyService taxonomyService) {
            _taxonomyService = taxonomyService;
        }

        protected override string Prefix { get { return "Taxonomy"; } }
        public Localizer T { get; set; }

        protected override DriverResult Display(TaxonomyPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_TaxonomyPart", () => {
                var taxonomyShape = shapeHelper.Taxonomy(ContentPart: part, ContentItem: part.ContentItem);
                var terms = _taxonomyService.GetTerms(part.ContentItem.Id);
                
                return shapeHelper.Parts_TaxonomyPart(Taxonomy: taxonomyShape);
            });
        }

        protected override DriverResult Editor(TaxonomyPart part, IUpdateModel updater, dynamic shapeHelper) {
            var existing = _taxonomyService.GetTaxonomyByName(part.Name);

            if (existing != null && existing.Record != part.Record) {
                updater.AddModelError("Title", T("A taxonomy with the same name already exists"));
            }
            
            // nothing to display for this part
            return null;
        }

        protected override void Exporting(TaxonomyPart part, ExportContentContext context) {
            if (part.IsInternal) {
                context.Exclude = true;
            }

            context.Element(part.PartDefinition.Name).SetAttributeValue("TermTypeName", part.TermTypeName);
        }

        protected override void Importing(TaxonomyPart part, ImportContentContext context) {
            part.TermTypeName = context.Attribute(part.PartDefinition.Name, "TermTypeName");
        }
    }
}