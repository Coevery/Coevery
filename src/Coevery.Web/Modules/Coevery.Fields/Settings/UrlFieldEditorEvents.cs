using System.Collections.Generic;
using Coevery.Entities.Settings;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class UrlFieldEditorEvents : FieldEditorEvents {
        public override IEnumerable<TemplateViewModel> FieldTypeDescriptor() {
            var model = string.Empty;
            yield return DisplayTemplate(model, "Url", null);
        }

        public override void UpdateFieldSettings(string fieldType, string fieldName, SettingsDictionary settingsDictionary, IUpdateModel updateModel) {
            if (fieldType != "UrlField") {
                return;
            }
            var model = new UrlFieldSettings();
            if (updateModel.TryUpdateModel(model, "UrlFieldSettings", null, null)) {
                UpdateSettings(model, settingsDictionary, "UrlFieldSettings");
                settingsDictionary["UrlFieldSettings.DefaultValue"] = model.DefaultValue;
                settingsDictionary["UrlFieldSettings.IsUnique"] = model.IsUnique.ToString();
            }
        }

        public override void UpdateFieldSettings(ContentPartFieldDefinitionBuilder builder, SettingsDictionary settingsDictionary) {
            if (builder.FieldType != "UrlField") {
                return;
            }

            var model = settingsDictionary.TryGetModel<UrlFieldSettings>();
            if (model != null) {
                UpdateSettings(model, builder, "UrlFieldSettings");
                builder.WithSetting("UrlFieldSettings.DefaultValue", model.DefaultValue);
                builder.WithSetting("UrlFieldSettings.IsUnique", model.IsUnique.ToString());
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "UrlField"
                || definition.FieldDefinition.Name == "UrlFieldCreate") {
                var model = definition.Settings.GetModel<UrlFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }
    }
}