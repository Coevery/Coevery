using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Title.Models;
using Coevery.Taxonomies.Fields;
using Coevery.Taxonomies.Services;
using JetBrains.Annotations;
using Coevery.Taxonomies.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Coevery.Taxonomies.Settings;

namespace Coevery.Taxonomies.Handlers {
    [UsedImplicitly]
    public class TaxonomyPartHandler : ContentHandler {
        public TaxonomyPartHandler(
            IRepository<TaxonomyPartRecord> repository, 
            ITaxonomyService taxonomyService,
            IContentDefinitionManager contentDefinitionManager) {

            int? previousId = null;

            Filters.Add(StorageFilter.For(repository));
            OnPublished<TaxonomyPart>((context, part) => {
                var previousTermTypeName = part.TermTypeName;
                taxonomyService.CreateTermContentType(part);

                if (previousId != null && part.Id != previousId) {

                    // remove previous term type
                    contentDefinitionManager.DeleteTypeDefinition(previousTermTypeName);

                    // update existing fields
                    foreach (var partDefinition in contentDefinitionManager.ListPartDefinitions()) {
                        foreach (var field in partDefinition.Fields) {
                            if (field.FieldDefinition.Name == typeof (TaxonomyField).Name) {

                                if (field.Settings.GetModel<TaxonomyFieldSettings>().TaxonomyId == previousId) {
                                    contentDefinitionManager.AlterPartDefinition(partDefinition.Name, 
                                        cfg => cfg.WithField(field.Name,
                                            builder => builder.WithSetting("TaxonomyFieldSettings.TaxonomyId", part.Id.ToString())));
                                }
                            }
                        }
                    }
                }
            });

            OnLoading<TaxonomyPart>( (context, part) => part.TermsField.Loader(x => taxonomyService.GetTerms(part.Id)));

            OnUpdating<TitlePart>((context, part) => {
                // if altering the title of a taxonomy, save the name
                if (part.As<TaxonomyPart>() != null) {
                    previousId = part.Id;
                }
            });
        }
    }
}