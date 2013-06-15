using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class CoeveryTextFieldListModeEvents : ContentDefinitionEditorEventsBase {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "CoeveryTextField") {
                var model = definition.Settings.GetModel<CoeveryTextFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "CoeveryTextField") {
                yield break;
            }

            var model = new CoeveryTextFieldSettings();
            if (updateModel.TryUpdateModel(model, "CoeveryTextFieldSettings", null, null)) {
                builder.WithSetting("CoeveryTextFieldSettings.HelpText", model.HelpText);
                builder.WithSetting("CoeveryTextFieldSettings.Required", model.Required.ToString());
                builder.WithSetting("CoeveryTextFieldSettings.ReadOnly", model.ReadOnly.ToString());
                builder.WithSetting("CoeveryTextFieldSettings.AlwaysInLayout", model.AlwaysInLayout.ToString());
                builder.WithSetting("CoeveryTextFieldSettings.IsSystemField", model.IsSystemField.ToString());
                builder.WithSetting("CoeveryTextFieldSettings.IsAudit", model.IsAudit.ToString());
                builder.WithSetting("CoeveryTextFieldSettings.MaxLength", model.MaxLength.ToString());
            }

            yield return DefinitionTemplate(model);
        }
    }
}