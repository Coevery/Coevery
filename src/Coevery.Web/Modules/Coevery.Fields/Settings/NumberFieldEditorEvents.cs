using System.Collections.Generic;
using Coevery.Entities.Settings;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class NumberFieldEditorEvents : FieldEditorEvents {
        public override IEnumerable<TemplateViewModel> FieldTypeDescriptor() {
            var model = string.Empty;
            yield return DisplayTemplate(model, "Number", null);
        }

        public override void UpdateFieldSettings(string fieldType, string fieldName, SettingsDictionary settingsDictionary, IUpdateModel updateModel) {
            if (fieldType != "NumberField") {
                return;
            }
            var model = new NumberFieldSettings();
            if (updateModel.TryUpdateModel(model, "NumberFieldSettings", null, null)) {
                UpdateSettings(model, settingsDictionary, "NumberFieldSettings");
                settingsDictionary["NumberFieldSettings.Length"] = model.Length.ToString("D");
                settingsDictionary["NumberFieldSettings.DecimalPlaces"] = model.DecimalPlaces.ToString("D");
                settingsDictionary["NumberFieldSettings.DefaultValue"] = model.DefaultValue.ToString();
            }
        }

        public override void UpdateFieldSettings(ContentPartFieldDefinitionBuilder builder, SettingsDictionary settingsDictionary) {
            if (builder.FieldType != "NumberField") {
                return;
            }

            var model = settingsDictionary.TryGetModel<NumberFieldSettings>();
            if (model != null) {
                UpdateSettings(model, builder, "NumberFieldSettings");
                builder.WithSetting("NumberFieldSettings.Length", model.Length.ToString());
                builder.WithSetting("NumberFieldSettings.DecimalPlaces", model.DecimalPlaces.ToString());
                builder.WithSetting("NumberFieldSettings.DefaultValue", model.DefaultValue.ToString());
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "NumberField"
                || definition.FieldDefinition.Name == "NumberFieldCreate") {
                var model = definition.Settings.GetModel<NumberFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }
    }
}