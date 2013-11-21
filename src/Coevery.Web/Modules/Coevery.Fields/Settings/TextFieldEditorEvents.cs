using System.Collections.Generic;
using Coevery.Entities.Settings;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class TextFieldEditorEvents : FieldEditorEvents {
        public override IEnumerable<TemplateViewModel> FieldTypeDescriptor() {
            var model = string.Empty;
            yield return DisplayTemplate(model, "Text", null);
        }

        public override void UpdateFieldSettings(string fieldType, string fieldName, SettingsDictionary settingsDictionary, IUpdateModel updateModel) {
            if (fieldType != "TextField") {
                return;
            }
            var model = new TextFieldSettings();
            if (updateModel.TryUpdateModel(model, "TextFieldSettings", null, null)) {
                UpdateSettings(model, settingsDictionary, "TextFieldSettings");
                settingsDictionary["TextFieldSettings.MaxLength"] = model.MaxLength.ToString();
                settingsDictionary["TextFieldSettings.PlaceHolderText"] = model.PlaceHolderText;
                settingsDictionary["TextFieldSettings.IsUnique"] = model.IsUnique.ToString();
            }
        }

        public override void UpdateFieldSettings(ContentPartFieldDefinitionBuilder builder, SettingsDictionary settingsDictionary) {
            if (builder.FieldType != "TextField") {
                return;
            }
            var model = settingsDictionary.TryGetModel<TextFieldSettings>();
            if (model != null) {
                UpdateSettings(model, builder, "TextFieldSettings");
                builder.WithSetting("TextFieldSettings.IsDisplayField", model.IsDisplayField.ToString());
                builder.WithSetting("TextFieldSettings.MaxLength", model.MaxLength.ToString());
                builder.WithSetting("TextFieldSettings.PlaceHolderText", model.PlaceHolderText);
                builder.WithSetting("TextFieldSettings.IsUnique", model.IsUnique.ToString());
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "TextField"
                || definition.FieldDefinition.Name == "TextFieldCreate") {
                var model = definition.Settings.GetModel<TextFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }
    }
}