using System.Collections.Generic;
using Coevery.Entities.Settings;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class DatetimeFieldEditorEvents : FieldEditorEvents {
        public override IEnumerable<TemplateViewModel> FieldTypeDescriptor() {
            var model = string.Empty;
            yield return DisplayTemplate(model, "Datetime", null);
        }

        public override void UpdateFieldSettings(string fieldType, string fieldName, SettingsDictionary settingsDictionary, IUpdateModel updateModel) {
            if (fieldType != "DatetimeField") {
                return;
            }
            var model = new DatetimeFieldSettings();
            if (updateModel.TryUpdateModel(model, "DatetimeFieldSettings", null, null)) {
                UpdateSettings(model, settingsDictionary, "DatetimeFieldSettings");
                settingsDictionary["DatetimeFieldSettings.DefaultValue"] = model.DefaultValue.ToString();
            }
        }

        public override void UpdateFieldSettings(ContentPartFieldDefinitionBuilder builder, SettingsDictionary settingsDictionary) {
            if (builder.FieldType != "DatetimeField") {
                return;
            }

            var model = settingsDictionary.TryGetModel<DatetimeFieldSettings>();
            if (model != null) {
                UpdateSettings(model, builder, "DatetimeFieldSettings");
                builder.WithSetting("DatetimeFieldSettings.DefaultValue", model.DefaultValue.ToString());
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "DatetimeField"
                || definition.FieldDefinition.Name == "DatetimeFieldCreate") {
                var model = definition.Settings.GetModel<DatetimeFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }
    }
}