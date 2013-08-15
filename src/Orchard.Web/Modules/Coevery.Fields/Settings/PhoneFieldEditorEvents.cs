using System.Collections.Generic;
using Coevery.Entities.Settings;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class PhoneFieldEditorEvents : FieldEditorEvents {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "PhoneField"
                || definition.FieldDefinition.Name == "PhoneFieldCreate") {
                var model = definition.Settings.GetModel<PhoneFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "PhoneField") {
                yield break;
            }

            var model = new PhoneFieldSettings();
            if (updateModel.TryUpdateModel(model, "PhoneFieldSettings", null, null)) {
                UpdateSettings(model, builder, "PhoneFieldSettings");
                builder.WithSetting("PhoneFieldSettings.DefaultValue", model.DefaultValue);
            }

            yield return DefinitionTemplate(model);
        }
    }
}