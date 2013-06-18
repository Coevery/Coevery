using System.Collections.Generic;
using System.Globalization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings {
    public class SelectFieldListModeEvents : FieldEditorEvents {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "SelectField") {
                var model = definition.Settings.GetModel<SelectFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "SelectField") {
                yield break;
            }

            var model = new SelectFieldSettings();
            if (updateModel.TryUpdateModel(model, "SelectFieldSettings", null, null)) {
                UpdateSettings(model, builder, "SelectFieldSettings");
            }

            yield return DefinitionTemplate(model);
        }
    }
}