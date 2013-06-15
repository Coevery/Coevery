using System.Collections.Generic;
using System.Globalization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class SelectFieldListModeEvents : ContentDefinitionEditorEventsBase {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "SelectField") {
                var model = definition.Settings.GetModel<SelectFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "SelectField") {
                yield break;
            }

            var model = new SelectFieldSettings();
            if (updateModel.TryUpdateModel(model, "SelectFieldSettings", null, null)) {
                builder.WithSetting("SelectFieldSettings.HelpText", model.HelpText);
                builder.WithSetting("SelectFieldSettings.Required", model.Required.ToString());
                builder.WithSetting("SelectFieldSettings.ReadOnly", model.ReadOnly.ToString());
                builder.WithSetting("SelectFieldSettings.AlwaysInLayout", model.AlwaysInLayout.ToString());
                builder.WithSetting("SelectFieldSettings.IsSystemField", model.IsSystemField.ToString());
                builder.WithSetting("SelectFieldSettings.IsAudit", model.IsAudit.ToString());
            }

            yield return DefinitionTemplate(model);
        }
    }
}