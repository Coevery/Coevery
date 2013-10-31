using Coevery.ContentManagement.MetaData.Builders;
using Coevery.ContentManagement.MetaData.Models;

namespace Coevery.Entities.Settings {
    public class FieldEditorEvents : ContentDefinitionEditorEventsBase {
        protected void UpdateSettings(FieldSettings model, ContentPartFieldDefinitionBuilder builder, string prefix) {
            model.HelpText = model.HelpText ?? string.Empty;
            builder.WithSetting(prefix + ".HelpText", model.HelpText);
            builder.WithSetting(prefix + ".Required", model.Required.ToString());
            builder.WithSetting(prefix + ".ReadOnly", model.ReadOnly.ToString());
            builder.WithSetting(prefix + ".AlwaysInLayout", model.AlwaysInLayout.ToString());
            builder.WithSetting(prefix + ".IsSystemField", model.IsSystemField.ToString());
            builder.WithSetting(prefix + ".IsAudit", model.IsAudit.ToString());
        }

        protected void UpdateSettings(FieldSettings model, SettingsDictionary settingsDictionary, string prefix) {
            model.HelpText = model.HelpText ?? string.Empty;
            settingsDictionary[prefix + ".HelpText"] = model.HelpText;
            settingsDictionary[prefix + ".Required"] = model.Required.ToString();
            settingsDictionary[prefix + ".ReadOnly"] = model.ReadOnly.ToString();
            settingsDictionary[prefix + ".AlwaysInLayout"] = model.AlwaysInLayout.ToString();
            settingsDictionary[prefix + ".IsSystemField"] = model.IsSystemField.ToString();
            settingsDictionary[prefix + ".IsAudit"] = model.IsAudit.ToString();
        }
    }
}