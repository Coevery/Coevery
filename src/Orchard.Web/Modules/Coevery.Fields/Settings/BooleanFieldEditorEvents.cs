using System.Collections.Generic;
using System.Globalization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class BooleanFieldEditorEvents : FieldEditorEvents {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "BooleanField"
                || definition.FieldDefinition.Name == "BooleanFieldCreate") {
                var model = definition.Settings.GetModel<BooleanFieldSettings>();
                yield return DefinitionTemplate(model);
            }
            else if (definition.FieldDefinition.Name == "BooleanFieldDisplay") {
                var model = definition.Settings.GetModel<BooleanFieldDisplaySettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "BooleanField") {
                yield break;
            }

            var model = new BooleanFieldSettings();
            if (updateModel.TryUpdateModel(model, "BooleanFieldSettings", null, null)) {
                UpdateSettings(model, builder, "BooleanFieldSettings");
                builder.WithSetting("BooleanFieldSettings.SelectionMode", model.SelectionMode.ToString());
                builder.WithSetting("BooleanFieldSettings.DependencyMode", model.DependencyMode.ToString());
                builder.WithSetting("BooleanFieldSettings.DefaultValue", model.DefaultValue.ToString());
            }

            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorCreate(ContentPartFieldDefinitionBuilder builder, string partName, IUpdateModel updateModel) {
            return PartFieldEditorUpdate(builder, updateModel);
        }
    }
}