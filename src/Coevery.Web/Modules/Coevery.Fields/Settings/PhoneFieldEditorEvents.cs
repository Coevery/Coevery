using System.Collections.Generic;
using Coevery.Entities.Settings;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class PhoneFieldEditorEvents : FieldEditorEvents {
        public override IEnumerable<TemplateViewModel> FieldTypeDescriptor() {
            var model = string.Empty;
            yield return DisplayTemplate(model, "Phone", null);
        }

        public override void UpdateFieldSettings(string fieldType, string fieldName, SettingsDictionary settingsDictionary, IUpdateModel updateModel) {
            if (fieldType != "PhoneField") {
                return;
            }
            var model = new PhoneFieldSettings();
            if (updateModel.TryUpdateModel(model, "PhoneFieldSettings", null, null)) {
                UpdateSettings(model, settingsDictionary, "PhoneFieldSettings");
                settingsDictionary["PhoneFieldSettings.DefaultValue"] = model.DefaultValue;
                settingsDictionary["PhoneFieldSettings.IsUnique"] = model.IsUnique.ToString();
            }
        }

        public override void UpdateFieldSettings(ContentPartFieldDefinitionBuilder builder, SettingsDictionary settingsDictionary) {
            if (builder.FieldType != "PhoneField") {
                return;
            }

            var model = settingsDictionary.TryGetModel<PhoneFieldSettings>();
            if (model != null) {
                UpdateSettings(model, builder, "PhoneFieldSettings");
                builder.WithSetting("PhoneFieldSettings.DefaultValue", model.DefaultValue);
                builder.WithSetting("PhoneFieldSettings.IsUnique", model.IsUnique.ToString());
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "PhoneField"
                || definition.FieldDefinition.Name == "PhoneFieldCreate") {
                var model = definition.Settings.GetModel<PhoneFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }
    }
}