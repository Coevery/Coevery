using System;
using System.Linq;
using Coevery.Taxonomies.Fields;
using Coevery.Taxonomies.Models;
using Coevery.Taxonomies.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Title.Models;
using Orchard.Data;

namespace Coevery.Taxonomies.Handlers {
    public class TermsPartHandler : ContentHandler {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;

        public TermsPartHandler(
            IContentDefinitionManager contentDefinitionManager,
            IRepository<TermsPartRecord> repository,
            ITaxonomyService taxonomyService,
            IContentManager contentManager) {
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;

            Filters.Add(StorageFilter.For(repository));

            // Tells how to load the field terms on demand, when a content item it loaded or when it has been created
            OnLoaded<TermsPart>((context, part) => InitializerTermsLoader(part));
            OnCreated<TermsPart>((context, part) => InitializerTermsLoader(part));

            OnIndexing<TermsPart>(
                (context, part) => {

                    foreach (var term in part.Terms) {
                        var termContentItem = context.ContentManager.Get(term.TermRecord.Id);
                        context.DocumentIndex.Add(term.Field, termContentItem.As<TitlePart>().Title).Analyze();
                        context.DocumentIndex.Add(term.Field + "-id", termContentItem.Id).Store();
                    }
                });
        }

        private void InitializerTermsLoader(TermsPart part) {
                foreach (var field in part.ContentItem.Parts.SelectMany(p => p.Fields).OfType<TaxonomyField>()) {
                    var tempField = field.Name;
                    var fieldTermRecordIds = part.Record.Terms.Where(t => t.Field == tempField).Select(tci => tci.TermRecord.Id);
                    field.TermsField.Loader(value => fieldTermRecordIds.Select(id => _contentManager.Get<TermPart>(id)).ToList());
                }

                part._termParts.Loader(value => 
                    part.Terms.Select(
                        x => new TermContentItemPart { Field = x.Field, TermPart = _contentManager.Get<TermPart>(x.TermRecord.Id) }
                        ));
        }

        protected override void Activating(ActivatingContentContext context) {
            base.Activating(context);

            // weld the TermsPart dynamically, if a field has been assigned to one of its parts
            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(context.ContentType);
            if (contentTypeDefinition == null) {
                return;
            }

            if (contentTypeDefinition.Parts.Any(
                part => part.PartDefinition.Fields.Any(
                    field => field.FieldDefinition.Name == typeof(TaxonomyField).Name))) {

                context.Builder.Weld<TermsPart>();
            }
        }
    }
}