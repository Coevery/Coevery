using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.Entities.Settings;
using Coevery.OptionSet.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Localization;

namespace Coevery.OptionSet.Settings {
    public class OptionSetFieldEditorEvents : FieldEditorEvents {
        private readonly IContentManager _contentManager;

        public OptionSetFieldEditorEvents(IContentManager contentManager) {
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "OptionSetField") {
                var model = definition.Settings.GetModel<OptionSetFieldSettings>();
                if (model.OptionSetId == 0) {
                    yield return DefinitionTemplate(model);
                }
                else {
                    var optionItems = _contentManager.Query<OptionItemPart, OptionItemPartRecord>()
                        .Where(x => x.OptionSetId == model.OptionSetId)
                        .WithQueryHints(new QueryHints().ExpandRecords<TitlePartRecord, CommonPartRecord>())
                        .List();
                    var options = optionItems.Aggregate(string.Empty, (current, next) =>
                        string.IsNullOrEmpty(current) ? next.Name : current + Environment.NewLine + next.Name);
                    model.Options = options;
                    var templateName = model.GetType().Name + ".Edit";
                    yield return DefinitionTemplate(model, templateName, model.GetType().Name);
                }
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "OptionSetField") {
                yield break;
            }

            var model = new OptionSetFieldSettings();

            if (updateModel.TryUpdateModel(model, "OptionSetFieldSettings", null, null)) {
                UpdateSettings(model, builder, "OptionSetFieldSettings");

                if (model.OptionSetId == 0) {
                    string[] options = (!String.IsNullOrWhiteSpace(model.Options)) ?
                        model.Options.Split(new[] {Environment.NewLine}, StringSplitOptions.None) : new string[] {};
                    var optionSetPart = _contentManager.New<OptionSetPart>("OptionSet");
                    optionSetPart.As<TitlePart>().Title = builder.Name;

                    _contentManager.Create(optionSetPart, VersionOptions.Published);

                    foreach (var option in options) {
                        var term = _contentManager.New<OptionItemPart>("OptionItem");
                        term.OptionSetId = optionSetPart.Id;
                        term.Name = option;
                        _contentManager.Create(term, VersionOptions.Published);
                    }
                    model.OptionSetId = optionSetPart.Id;
                }

                builder
                    .WithSetting("OptionSetFieldSettings.OptionSetId", model.OptionSetId.ToString())
                    .WithSetting("OptionSetFieldSettings.ListMode", model.ListMode.ToString());
            }

            yield return DefinitionTemplate(model);
        }
    }
}