using System.Collections.Generic;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;

namespace Coevery.Core.Common.OwnerEditor {
    public class OwnerEditorSettings {

        public OwnerEditorSettings() {
            // owner editor should is displayed by default
            ShowOwnerEditor = true;
        }

        public bool ShowOwnerEditor { get; set; }
    }

    public class OwnerEditorSettingsEvents : ContentDefinitionEditorEventsBase {
        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition) {
            if (definition.PartDefinition.Name == "CommonPart") {
                var model = definition.Settings.GetModel<OwnerEditorSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel upOwnerModel) {
            if (builder.Name == "CommonPart") {
                var model = new OwnerEditorSettings();
                if (upOwnerModel.TryUpdateModel(model, "OwnerEditorSettings", null, null)) {
                    builder.WithSetting("OwnerEditorSettings.ShowOwnerEditor", model.ShowOwnerEditor.ToString());
                }
                yield return DefinitionTemplate(model);
            }
        }
    }
}