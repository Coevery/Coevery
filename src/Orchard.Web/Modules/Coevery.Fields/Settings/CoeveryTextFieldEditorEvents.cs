using System.Collections.Generic;
using Coevery.Entities.Settings;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class CoeveryTextFieldEditorEvents : FieldEditorEvents {
        public override IEnumerable<TemplateViewModel> FieldDescriptor() {
            var model = string.Empty;
            yield return DisplayTemplate(model, "CoeveryText", null);
        }

        public override void UpdateFieldSettings(string fieldType, string fieldName, SettingsDictionary settingsDictionary, IUpdateModel updateModel) {
            if (fieldType != "CoeveryTextField") {
                return;
            }
            var model = new CoeveryTextFieldSettings();
            if (updateModel.TryUpdateModel(model, "CoeveryTextFieldSettings", null, null)) {
                UpdateSettings(model, settingsDictionary, "CoeveryTextFieldSettings");
                settingsDictionary["CoeveryTextFieldSettings.MaxLength"] = model.MaxLength.ToString();
                settingsDictionary["CoeveryTextFieldSettings.PlaceHolderText"] = model.PlaceHolderText;
            }
        }

        public override void UpdateFieldSettings(ContentPartFieldDefinitionBuilder builder, SettingsDictionary settingsDictionary) {
            if (builder.FieldType != "CoeveryTextField") {
                return;
            }
            var model = settingsDictionary.TryGetModel<CoeveryTextFieldSettings>();
            if (model != null) {
                UpdateSettings(model, builder, "CoeveryTextFieldSettings");
                builder.WithSetting("CoeveryTextFieldSettings.IsDispalyField", model.IsDispalyField.ToString());
                builder.WithSetting("CoeveryTextFieldSettings.MaxLength", model.MaxLength.ToString());
                builder.WithSetting("CoeveryTextFieldSettings.PlaceHolderText", model.PlaceHolderText);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "CoeveryTextField"
                || definition.FieldDefinition.Name == "CoeveryTextFieldCreate") {
                var model = definition.Settings.GetModel<CoeveryTextFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }
    }
}