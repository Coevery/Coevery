using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Coevery.Fields.Records;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;
using Orchard.Localization;

namespace Coevery.Fields.Settings
{
    public class SelectFieldListModeEvents : FieldEditorEvents
    {
        private readonly IRepository<OptionItemRecord> _optionItemRepository;
        private readonly IRepository<ContentPartDefinitionRecord> _partDefinitionRepository;
        private readonly Localizer _t;

        public SelectFieldListModeEvents()
        {
            _t = NullLocalizer.Instance;
        }

        public SelectFieldListModeEvents(
            IRepository<OptionItemRecord> optionItemRepository,
            IRepository<ContentPartDefinitionRecord> partDefinitionRepository)
        {
            _optionItemRepository = optionItemRepository;
            _partDefinitionRepository = partDefinitionRepository;
            _t = NullLocalizer.Instance;
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition)
        {
            if (definition.FieldDefinition.Name == "SelectField" ||
                definition.FieldDefinition.Name == "SelectFieldCreate")
            {
                var model = definition.Settings.GetModel<SelectFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel)
        {
            if (builder.FieldType != "SelectField")
            {
                yield break;
            }

            var model = new SelectFieldSettings();
            if (updateModel.TryUpdateModel(model, "SelectFieldSettings", null, null))
            {
                UpdateSettings(model, builder, "SelectFieldSettings");
                builder.WithSetting("SelectFieldSettings.ItemCount", model.ItemCount.ToString());
                builder.WithSetting("SelectFieldSettings.DisplayLines", model.DisplayLines.ToString());
                builder.WithSetting("SelectFieldSettings.DisplayOption", model.DisplayOption.ToString());
                builder.WithSetting("SelectFieldSettings.LabelsStr", model.LabelsStr);
                builder.WithSetting("SelectFieldSettings.DefaultValue", model.DefaultValue.ToString());
                builder.WithSetting("SelectFieldSettings.SelectCount", model.SelectCount.ToString());
            }

            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorCreate(ContentPartFieldDefinitionBuilder builder, string typeName, IUpdateModel updateModel)
        {
            if (builder.FieldType != "SelectField")
            {
                yield break;
            }

            var model = new SelectFieldSettings();
            if (updateModel.TryUpdateModel(model, "SelectFieldSettings", null, null))
            {
                var field = _partDefinitionRepository.Table.Single(x => x.Name == typeName)
                    .ContentPartFieldDefinitionRecords.Single(x => x.Name == builder.Name);
                var labels = model.LabelsStr.Split(new string[] { "\r\n",";" }, StringSplitOptions.RemoveEmptyEntries);

                //Basic Validation, should be replaced later       
                if (model.SelectCount < 1 || model.ItemCount != labels.Length || model.DisplayLines > model.ItemCount
                    || model.DefaultValue > model.DisplayLines) 
                {
                    updateModel.AddModelError("Settings", _t("The setting values have conflicts."));
                    yield break;
                }

                int idIndex = 0;
                foreach (var label in labels)
                {
                    idIndex++;
                    var option = new OptionItemRecord
                    {
                        Value = label,
                        ContentPartFieldDefinitionRecord = field,
                        IsDefault = (idIndex == model.DefaultValue)
                    };
                    _optionItemRepository.Create(option);
                }
                UpdateSettings(model, builder, "SelectFieldSettings");
            }

            yield return DefinitionTemplate(model);
        }
    }
}