using System.Collections.Generic;
using Coevery.Entities.Settings;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class EmailFieldEditorEvents : FieldEditorEvents {
        public override IEnumerable<TemplateViewModel> FieldDescriptor() {
            var model = string.Empty;
            yield return DisplayTemplate(model, "Email", null);
        }

        public override void UpdateFieldSettings(string fieldType, string fieldName, SettingsDictionary settingsDictionary, IUpdateModel updateModel) {
            if (fieldType != "EmailField") {
                return;
            }
            var model = new EmailFieldSettings();
            if (updateModel.TryUpdateModel(model, "EmailFieldSettings", null, null)) {
                UpdateSettings(model, settingsDictionary, "EmailFieldSettings");
                settingsDictionary["EmailFieldSettings.DefaultValue"] = model.DefaultValue;
            }
        }

        public override void UpdateFieldSettings(ContentPartFieldDefinitionBuilder builder, SettingsDictionary settingsDictionary) {
            if (builder.FieldType != "EmailField") {
                return;
            }

            var model = settingsDictionary.TryGetModel<EmailFieldSettings>();
            if (model != null) {
                UpdateSettings(model, builder, "EmailFieldSettings");
                builder.WithSetting("EmailFieldSettings.DefaultValue", model.DefaultValue);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "EmailField"
                || definition.FieldDefinition.Name == "EmailFieldCreate") {
                var model = definition.Settings.GetModel<EmailFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }
    }
}