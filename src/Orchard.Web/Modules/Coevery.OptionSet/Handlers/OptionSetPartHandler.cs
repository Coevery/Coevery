using Coevery.OptionSet.Fields;
using Coevery.OptionSet.Models;
using Coevery.OptionSet.Services;
using Coevery.OptionSet.Settings;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Title.Models;
using Orchard.Data;

namespace Coevery.OptionSet.Handlers {
    [UsedImplicitly]
    public class OptionSetPartHandler : ContentHandler {
        public OptionSetPartHandler(
            IRepository<OptionSetPartRecord> repository, 
            IOptionSetService optionSetService,
            IContentDefinitionManager contentDefinitionManager) {

            int? previousId = null;

            Filters.Add(StorageFilter.For(repository));
            OnPublished<OptionSetPart>((context, part) => {
                var previousTermTypeName = part.TermTypeName;

                if (previousId != null && part.Id != previousId) {

                    // remove previous term type
                    contentDefinitionManager.DeleteTypeDefinition(previousTermTypeName);

                    // update existing fields
                    foreach (var partDefinition in contentDefinitionManager.ListPartDefinitions()) {
                        foreach (var field in partDefinition.Fields) {
                            if (field.FieldDefinition.Name == typeof (OptionSetField).Name) {

                                if (field.Settings.GetModel<OptionSetFieldSettings>().OptionSetId == previousId) {
                                    contentDefinitionManager.AlterPartDefinition(partDefinition.Name, 
                                        cfg => cfg.WithField(field.Name,
                                            builder => builder.WithSetting("TaxonomyFieldSettings.TaxonomyId", part.Id.ToString())));
                                }
                            }
                        }
                    }
                }
            });

            OnLoading<OptionSetPart>((context, part) => part.OptionItemsField.Loader(x => optionSetService.GetOptionItems(part.Id)));

            OnUpdating<TitlePart>((context, part) => {
                // if altering the title of a taxonomy, save the name
                if (part.As<OptionSetPart>() != null) {
                    previousId = part.Id;
                }
            });
        }
    }
}