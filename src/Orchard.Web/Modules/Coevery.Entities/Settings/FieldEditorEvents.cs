using Coevery.Entities.Settings;
using Orchard.ContentManagement.MetaData.Builders;

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
    }
}