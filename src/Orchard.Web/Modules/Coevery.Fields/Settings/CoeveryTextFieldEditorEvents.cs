using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class CoeveryTextFieldEditorEvents : FieldEditorEvents {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "CoeveryTextField"
                || definition.FieldDefinition.Name == "CoeveryTextFieldCreate") {
                var model = definition.Settings.GetModel<CoeveryTextFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "CoeveryTextField") {
                yield break;
            }

            var model = new CoeveryTextFieldSettings();
            if (updateModel.TryUpdateModel(model, "CoeveryTextFieldSettings", null, null)) {
                UpdateSettings(model, builder, "CoeveryTextFieldSettings");
                builder.WithSetting("CoeveryTextFieldSettings.MaxLength", model.MaxLength.ToString());
            }

            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorCreate(ContentPartFieldDefinitionBuilder builder, string partName, IUpdateModel updateModel) {
            return PartFieldEditorUpdate(builder, updateModel);
        }
    }
}