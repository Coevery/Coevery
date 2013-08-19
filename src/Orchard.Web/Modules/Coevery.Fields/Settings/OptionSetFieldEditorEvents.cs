using System;
using System.Collections.Generic;
using Coevery.Entities.Settings;
using Coevery.Fields.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;
using Orchard.Localization;

namespace Coevery.Fields.Settings {
    public class OptionSetFieldListModeEvents : FieldEditorEvents {
        private readonly IOptionItemService _optionItemService;

        public OptionSetFieldListModeEvents(IOptionItemService optionItemService) {
            _optionItemService = optionItemService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "OptionSetField" ||
                definition.FieldDefinition.Name == "OptionSetFieldCreate") {
                var model = definition.Settings.GetModel<OptionSetFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "OptionSetField") {
                yield break;
            }

            var model = new OptionSetFieldSettings();
            if (updateModel.TryUpdateModel(model, "OptionSetFieldSettings", null, null)) {
                var labels = model.LabelsStr.Split(OptionSetFieldSettings.LabelSeperator, StringSplitOptions.RemoveEmptyEntries);
                //model.FieldSettingId = _optionItemService.InitializeField(typeName, builder.Name, labels, model.DefaultValue);
                var itemCount = labels.Length;
                ValidateSettings(updateModel, model, itemCount);
                UpdateSettings(model, builder, "OptionSetFieldSettings");
                builder.WithSetting("OptionSetFieldSettings.DisplayLines", model.DisplayLines.ToString());
                builder.WithSetting("OptionSetFieldSettings.DisplayOption", model.DisplayOption.ToString());
                builder.WithSetting("OptionSetFieldSettings.SelectCount", model.SelectCount.ToString());
            }

            yield return DefinitionTemplate(model);
        }

        private void ValidateSettings(IUpdateModel updateModel, OptionSetFieldSettings model, int itemCount) {
            if (model.OptionSetId <= 0) {
        //public override IEnumerable<TemplateViewModel> PartFieldEditorCreate(ContentPartFieldDefinitionBuilder builder, string typeName, IUpdateModel updateModel) {
        //    if (builder.FieldType != "SelectField") {
        //        yield break;
        //    }

        //    var model = new SelectFieldSettings();
        //    if (updateModel.TryUpdateModel(model, "SelectFieldSettings", null, null)) {
        //        var labels = model.LabelsStr.Split(SelectFieldSettings.LabelSeperator, StringSplitOptions.RemoveEmptyEntries);
        //        model.FieldSettingId = _optionItemService.InitializeField(typeName, builder.Name, labels, model.DefaultValue);
        //        var itemCount = labels.Length;
        //        ValidateSettings(updateModel, model, itemCount);

        //        UpdateSettings(model, builder, "SelectFieldSettings");
        //        builder.WithSetting("SelectFieldSettings.DisplayLines", model.DisplayLines.ToString());
        //        builder.WithSetting("SelectFieldSettings.DisplayOption", model.DisplayOption.ToString());
        //        builder.WithSetting("SelectFieldSettings.FieldSettingId", model.FieldSettingId.ToString());
        //        builder.WithSetting("SelectFieldSettings.SelectCount", model.SelectCount.ToString());
        //        builder.WithSetting("SelectFieldSettings.DependencyMode", model.DependencyMode.ToString());
        //    }

        //    yield return DefinitionTemplate(model);
        //}

        private void ValidateSettings(IUpdateModel updateModel, OptionSetFieldSettings model, int itemCount) {
            if (model.OptionSetId <= 0) {
                updateModel.AddModelError("OptionSetSettings", T("Create option items failed."));
            }

            //Only check when creating
            if (model.DefaultValue > itemCount || model.DefaultValue < 0) {
                updateModel.AddModelError("OptionSetSettings", T("DefaultValue is out of range."));
            }

            if (itemCount == 0) {
                updateModel.AddModelError("OptionSetSettings", T("No valid label exists."));
            }

            //Setting value range
            if (model.SelectCount < 1 || model.SelectCount > itemCount) {
                updateModel.AddModelError("OptionSetSettings", T("SelectCount is out of range."));
            }
            if (model.DisplayLines < 1 || model.DisplayLines > itemCount) {
                updateModel.AddModelError("OptionSetSettings", T("DisplayLines is out of range."));
            }

            //Display option and select count match
            if ((model.DisplayOption == SelectionMode.Checkbox && model.SelectCount == 1)
                || (model.DisplayOption == SelectionMode.Radiobutton && model.SelectCount > 1)) {
                    updateModel.AddModelError("OptionSetSettings", T("The display option and select count don't match."));
            }
        }
    }
}