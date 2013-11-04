using System.Collections.Generic;
using Coevery.Entities.Settings;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class CurrencyFieldEditorEvents : FieldEditorEvents {
        public override IEnumerable<TemplateViewModel> FieldTypeDescriptor() {
            var model = string.Empty;
            yield return DisplayTemplate(model, "Currency", null);
        }

        public override void UpdateFieldSettings(string fieldType, string fieldName, SettingsDictionary settingsDictionary, IUpdateModel updateModel) {
            if (fieldType != "CurrencyField") {
                return;
            }
            var model = new CurrencyFieldSettings();
            if (updateModel.TryUpdateModel(model, "CurrencyFieldSettings", null, null)) {
                UpdateSettings(model, settingsDictionary, "CurrencyFieldSettings");
                settingsDictionary["CurrencyFieldSettings.Length"] = model.Length.ToString("D");
                settingsDictionary["CurrencyFieldSettings.DecimalPlaces"] = model.DecimalPlaces.ToString("D");
                settingsDictionary["CurrencyFieldSettings.DefaultValue"] = model.DefaultValue.ToString();
            }
        }

        public override void UpdateFieldSettings(ContentPartFieldDefinitionBuilder builder, SettingsDictionary settingsDictionary) {
            if (builder.FieldType != "CurrencyField") {
                return;
            }

            var model = settingsDictionary.TryGetModel<CurrencyFieldSettings>();
            if (model != null) {
                UpdateSettings(model, builder, "CurrencyFieldSettings");
                builder.WithSetting("CurrencyFieldSettings.Length", model.Length.ToString());
                builder.WithSetting("CurrencyFieldSettings.DecimalPlaces", model.DecimalPlaces.ToString());
                builder.WithSetting("CurrencyFieldSettings.DefaultValue", model.DefaultValue.ToString());
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "CurrencyField"
                || definition.FieldDefinition.Name == "CurrencyFieldCreate") {
                var model = definition.Settings.GetModel<CurrencyFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }
    }
}