using System.Collections.Generic;
using System.Globalization;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Fields.Settings
{
    public class NumberFieldEditorEvents : FieldEditorEvents
    {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition)
        {
            if (definition.FieldDefinition.Name == "NumberField"
                || definition.FieldDefinition.Name == "NumberFieldCreate")
            {
                var model = definition.Settings.GetModel<NumberFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel)
        {
            if (builder.FieldType != "NumberField")
            {
                yield break;
            }

            var model = new NumberFieldSettings();
            if (updateModel.TryUpdateModel(model, "NumberFieldSettings", null, null))
            {
                UpdateSettings(model, builder, "NumberFieldSettings");
                builder.WithSetting("NumberFieldSettings.Length", model.Length.ToString());
                builder.WithSetting("NumberFieldSettings.DecimalPlaces", model.DecimalPlaces.ToString());
                builder.WithSetting("NumberFieldSettings.DefaultValue", model.DefaultValue.ToString());
            }

            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorCreate(ContentPartFieldDefinitionBuilder builder, string partName, IUpdateModel updateModel)
        {
            return PartFieldEditorUpdate(builder, updateModel);
        }
    }
}