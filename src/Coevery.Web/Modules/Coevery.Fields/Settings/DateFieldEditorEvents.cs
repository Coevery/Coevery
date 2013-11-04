using System.Collections.Generic;
using Coevery.Entities.Settings;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class DateFieldEditorEvents : FieldEditorEvents {
        public override IEnumerable<TemplateViewModel> FieldTypeDescriptor() {
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