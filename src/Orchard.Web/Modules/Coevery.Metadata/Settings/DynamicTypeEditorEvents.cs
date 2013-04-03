
using System.Collections.Generic;
using Coevery.Metadata.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Dynamic.Settings
{
    public class DynamicTypeEditorEvents : ContentDefinitionEditorEventsBase
    {
        public override IEnumerable<TemplateViewModel> TypeEditor(ContentTypeDefinition definition)
        {
            var settings = definition.Settings.GetModel<DynamicTypeSettings>();
            var model = new DynamicTypeSettingsViewModel
            {
                IsDeployed = settings.IsDeployed
            };

            yield return DefinitionTemplate(model);
        }

        public override IEnumerable<TemplateViewModel> TypeEditorUpdate(ContentTypeDefinitionBuilder builder, IUpdateModel updateModel)
        {
            var model = new DynamicTypeSettingsViewModel();
            updateModel.TryUpdateModel(model, "DynamicTypeSettingsViewModel", null, null);

            builder.WithSetting("DynamicTypeSettings.IsDeployed", model.IsDeployed.ToString());

            yield return DefinitionTemplate(model);
        }
    }
}
