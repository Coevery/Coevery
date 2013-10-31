using System.Collections.Generic;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;
using Coevery.Environment.Extensions;
using Coevery.Localization;

namespace Coevery.Scripting.CSharp.Settings {
    [CoeveryFeature("Coevery.Scripting.CSharp.Validation")]
    public class ScriptValidationPartSettingsEvents : ContentDefinitionEditorEventsBase {

        public Localizer T { get; set; }

        public override IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition) {
            if (definition.PartDefinition.Name != "ScriptValidationPart")
                yield break;

            var settings = definition.Settings.GetModel<ScriptValidationPartSettings>();

            yield return DefinitionTemplate(settings);
        }

        public override IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.Name != "ScriptValidationPart")
                yield break;

            var settings = new ScriptValidationPartSettings();

            if (updateModel.TryUpdateModel(settings, "ScriptValidationPartSettings", null, null)) {
                builder.WithSetting("ScriptValidationPartSettings.Script", settings.Script);
            }

            yield return DefinitionTemplate(settings);
        }
    }
}
