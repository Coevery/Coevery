using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class DatetimeFieldEditorEvents : FieldEditorEvents {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "DatetimeField"
                || definition.FieldDefinition.Name == "DatetimeFieldCreate") {
                var model = definition.Settings.GetModel<DatetimeFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "DatetimeField") {
                yield break;
            }

            var model = new DatetimeFieldSettings();
            if (updateModel.TryUpdateModel(model, "DatetimeFieldSettings", null, null)) {
                UpdateSettings(model, builder, "DatetimeFieldSettings");
                builder.WithSetting("DatetimeFieldSettings.DefaultValue", model.DefaultValue.ToString("MM/dd/yyyy hh:mm tt"));
            }

            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorCreate(ContentPartFieldDefinitionBuilder builder, string partName, IUpdateModel updateModel) {
            return PartFieldEditorUpdate(builder, updateModel);
        }
    }
}
