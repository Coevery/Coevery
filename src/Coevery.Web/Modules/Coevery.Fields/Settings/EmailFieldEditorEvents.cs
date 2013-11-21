using System.Collections.Generic;
using Coevery.Entities.Settings;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class EmailFieldEditorEvents : FieldEditorEvents {
        public override IEnumerable<TemplateViewModel> FieldTypeDescriptor() {
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
                settingsDictionary["EmailFieldSettings.IsUnique"] = model.IsUnique.ToString();
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
                builder.WithSetting("EmailFieldSettings.IsUnique", model.IsUnique.ToString());
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