using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.Taxonomies.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using Coevery.Taxonomies.Services;
using Coevery.Entities.Settings;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Localization;

namespace Coevery.Taxonomies.Settings {
    public class TaxonomyFieldEditorEvents : FieldEditorEvents {
        private readonly ITaxonomyService _taxonomyService;
        private readonly IContentManager _contentManager;

        public TaxonomyFieldEditorEvents(ITaxonomyService taxonomyService, 
            IContentManager contentManager) {
            _taxonomyService = taxonomyService;
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "TaxonomyField") {
                var model = definition.Settings.GetModel<TaxonomyFieldSettings>();
                var terms = _contentManager.Query<TermPart, TermPartRecord>()
                    .Where(x => x.TaxonomyId == model.TaxonomyId)
                    .WithQueryHints(new QueryHints().ExpandRecords<TitlePartRecord, CommonPartRecord>())
                    .List();
                var options = terms.Aggregate(string.Empty, (current, next) =>
                    string.IsNullOrEmpty(current) ? next.Name : current + Environment.NewLine + next.Name);
                model.Options = options;
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "TaxonomyField") {
                yield break;
            }

            var model = new TaxonomyFieldSettings();

            if (updateModel.TryUpdateModel(model, "TaxonomyFieldSettings", null, null)) {
                UpdateSettings(model, builder, "TaxonomyFieldSettings");

                if (model.TaxonomyId == 0) {
                    string[] options = (!String.IsNullOrWhiteSpace(model.Options)) ?
                        model.Options.Split(new[] {Environment.NewLine}, StringSplitOptions.None) : new string[] {};
                    var taxonomyPart = _contentManager.New<TaxonomyPart>("Taxonomy");
                    taxonomyPart.As<TitlePart>().Title = builder.Name;
                    _contentManager.Create(taxonomyPart, VersionOptions.Published);

                    foreach (var option in options) {
                        var term = _contentManager.New<TermPart>("Term");
                        term.TaxonomyId = taxonomyPart.Id;
                        term.Name = option;
                        _contentManager.Create(term, VersionOptions.Published);
                    }
                    model.TaxonomyId = taxonomyPart.Id;
                }

                builder
                    .WithSetting("TaxonomyFieldSettings.TaxonomyId", model.TaxonomyId.ToString())
                    .WithSetting("TaxonomyFieldSettings.LeavesOnly", model.LeavesOnly.ToString())
                    .WithSetting("TaxonomyFieldSettings.Required", model.Required.ToString())
                    .WithSetting("TaxonomyFieldSettings.SingleChoice", model.SingleChoice.ToString())
                    .WithSetting("TaxonomyFieldSettings.ListMode", model.ListMode.ToString());
            }

            yield return DefinitionTemplate(model);
        }
    }
}