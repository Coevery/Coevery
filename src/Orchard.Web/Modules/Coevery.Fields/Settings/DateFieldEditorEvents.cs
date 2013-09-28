using System.Collections.Generic;
using Coevery.Entities.Settings;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class DateFieldEditorEvents : FieldEditorEvents {
        public override IEnumerable<TemplateViewModel> FieldDescriptor() {
            var model = string.Empty;
            yield return DisplayTemplate(model, "Date", null);
        }

        public override void UpdateFieldSettings(string fieldType, string fieldName, SettingsDictionary settingsDictionary, IUpdateModel updateModel) {
            if (fieldType != "DateField") {
                return;
            }
            var model = new DateFieldSettings();
            if (updateModel.TryUpdateModel(model, "DateFieldSettings", null, null)) {
                UpdateSettings(model, settingsDictionary, "DateFieldSettings");
                settingsDictionary["DateFieldSettings.DefaultValue"] = model.DefaultValue.ToString();
            }
        }

        public override void UpdateFieldSettings(ContentPartFieldDefinitionBuilder builder, SettingsDictionary settingsDictionary) {
            if (builder.FieldType != "DateField") {
                return;
            }

            var model = settingsDictionary.TryGetModel<DateFieldSettings>();
            if (model != null) {
                UpdateSettings(model, builder, "DateFieldSettings");
                builder.WithSetting("DateFieldSettings.DefaultValue", model.DefaultValue.ToString());
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "DateField"
                || definition.FieldDefinition.Name == "DateFieldCreate") {
                var model = definition.Settings.GetModel<DateFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }
    }
}