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

namespace Coevery.Fields.Settings {
    public class SelectFieldListModeEvents : FieldEditorEvents {
        private readonly IRepository<OptionItemRecord> _optionItemRepository;
        private readonly IRepository<ContentPartDefinitionRecord> _partDefinitionRepository;

        public SelectFieldListModeEvents(
            IRepository<OptionItemRecord> optionItemRepository,
            IRepository<ContentPartDefinitionRecord> partDefinitionRepository) {
            _optionItemRepository = optionItemRepository;
            _partDefinitionRepository = partDefinitionRepository;
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "SelectFieldCreate") {
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

        public override IEnumerable<TemplateViewModel> PartFieldEditorCreate(ContentPartFieldDefinitionBuilder builder, string typeName, IUpdateModel updateModel) {
            if (builder.FieldType != "SelectField") {
                yield break;
            }

            var model = new SelectFieldSettings();
            if (updateModel.TryUpdateModel(model, "SelectFieldSettings", null, null)) {
                var field = _partDefinitionRepository.Table.Single(x => x.Name == typeName)
                    .ContentPartFieldDefinitionRecords.Single(x => x.Name == builder.Name);
                var items = model.ItemsStr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in items) {
                    var option = new OptionItemRecord {
                        Value = item,
                        ContentPartFieldDefinitionRecord = field
                    };
                    _optionItemRepository.Create(option);
                }
                UpdateSettings(model, builder, "SelectFieldSettings");
            }

            yield return DefinitionTemplate(model);
        }
    }
}