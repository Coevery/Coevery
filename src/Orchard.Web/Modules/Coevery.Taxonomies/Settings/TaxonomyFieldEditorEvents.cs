using System.Collections.Generic;
using System.Linq;
using Coevery.Taxonomies.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using Coevery.Taxonomies.Services;
using Coevery.Entities.Settings;
using Orchard.Localization;

namespace Coevery.Taxonomies.Settings {
    public class TaxonomyFieldEditorEvents : FieldEditorEvents {
        private readonly ITaxonomyService _taxonomyService;

        public TaxonomyFieldEditorEvents(ITaxonomyService taxonomyService) {
            _taxonomyService = taxonomyService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "TaxonomyField"
                || definition.FieldDefinition.Name == "TaxonomyFieldCreate") {
                var model = definition.Settings.GetModel<TaxonomyFieldSettings>();
                model.Taxonomies = _taxonomyService.GetTaxonomies();
                if (!string.IsNullOrWhiteSpace(model.Taxonomy)) {
                    model.TaxonomyId = _taxonomyService.GetTaxonomyByName(model.Taxonomy).Id;
                }
                if (!model.Taxonomies.Any()) {
                    model.Taxonomies = null;
                }
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "TaxonomyField") {
                yield break;
            }

            var model = new TaxonomyFieldSettings();

            if (updateModel.TryUpdateModel(model, "TaxonomyFieldSettings", null, null)) {
                if (!string.IsNullOrWhiteSpace(model.Terms)) {
                    var taxonomy = _taxonomyService.GetTaxonomyByName(model.Taxonomy);
                    if (taxonomy == null || taxonomy.Id == 0) {
                        taxonomy = new TaxonomyPart {
                            IsInternal = false,
                            TermTypeName = model.Taxonomy
                        };
                        _taxonomyService.CreateTermContentType(taxonomy);
                        if (!_taxonomyService.GenerateTermsFromImport(taxonomy.Id, model.Terms)) {
                            yield break;
                        }
                    }
                }
                builder
                    .WithSetting("TaxonomyFieldSettings.Taxonomy", model.Taxonomy)
                    .WithSetting("TaxonomyFieldSettings.LeavesOnly", model.LeavesOnly.ToString())
                    .WithSetting("TaxonomyFieldSettings.SingleChoice", model.SingleChoice.ToString())
                    .WithSetting("TaxonomyFieldSettings.AllowCustomTerms", model.AllowCustomTerms.ToString());
            }

            yield return DefinitionTemplate(model);
        }
    }
}