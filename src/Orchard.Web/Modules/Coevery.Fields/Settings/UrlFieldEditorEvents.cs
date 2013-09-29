using System.Collections.Generic;
using Coevery.Entities.Settings;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class UrlFieldEditorEvents : FieldEditorEvents {
        public override IEnumerable<TemplateViewModel> FieldDescriptor() {
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