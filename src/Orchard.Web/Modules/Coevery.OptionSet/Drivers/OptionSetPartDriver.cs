using Coevery.OptionSet.Models;
using Coevery.OptionSet.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;

namespace Coevery.OptionSet.Drivers {
    public class OptionSetPartDriver : ContentPartDriver<OptionSetPart> {
        private readonly IOptionSetService _optionSetService;

        public OptionSetPartDriver(IOptionSetService optionSetService) {
            _optionSetService = optionSetService;
        }

        protected override string Prefix { get { return "OptionSet"; } }
        public Localizer T { get; set; }

        protected override DriverResult Display(OptionSetPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_OptionSetPart", () => {
                var taxonomyShape = shapeHelper.Taxonomy(ContentPart: part, ContentItem: part.ContentItem);
                var terms = _optionSetService.GetOptionItems(part.ContentItem.Id);

                return shapeHelper.Parts_OptionSetPart(Taxonomy: taxonomyShape);
            });
        }

        protected override DriverResult Editor(OptionSetPart part, IUpdateModel updater, dynamic shapeHelper) {
            var existing = _optionSetService.GetOptionSetByName(part.Name);

            if (existing != null && existing.Record != part.Record) {
                updater.AddModelError("Title", T("A taxonomy with the same name already exists"));
            }
            
            // nothing to display for this part
            return null;
        }

        protected override void Exporting(OptionSetPart part, ExportContentContext context) {
            if (part.IsInternal) {
                context.Exclude = true;
            }

            context.Element(part.PartDefinition.Name).SetAttributeValue("TermTypeName", part.TermTypeName);
        }

        protected override void Importing(OptionSetPart part, ImportContentContext context) {
            part.TermTypeName = context.Attribute(part.PartDefinition.Name, "TermTypeName");
        }
    }
}