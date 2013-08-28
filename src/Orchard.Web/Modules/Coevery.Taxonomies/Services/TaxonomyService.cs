using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Coevery.Taxonomies.Fields;
using Coevery.Taxonomies.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;
using Orchard.Data;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.UI.Notify;
using Orchard.Utility.Extensions;

namespace Coevery.Taxonomies.Services {
    public class TaxonomyService : ITaxonomyService {
        private readonly IRepository<TermContentItem> _termContentItemRepository;
        private readonly IContentManager _contentManager;
        private readonly INotifier _notifier;
        private readonly IAuthorizationService _authorizationService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IOrchardServices _services;

        public TaxonomyService(
            IRepository<TermContentItem> termContentItemRepository,
            IContentManager contentManager,
            INotifier notifier,
            IContentDefinitionManager contentDefinitionManager,
            IAuthorizationService authorizationService,
            IOrchardServices services) {
            _termContentItemRepository = termContentItemRepository;
            _contentManager = contentManager;
            _notifier = notifier;
            _authorizationService = authorizationService;
            _contentDefinitionManager = contentDefinitionManager;
            _services = services;

            Logger = NullLogger.Instance;
            T = NullLocalizer.Instance;
        }

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }

        public IEnumerable<TaxonomyPart> GetTaxonomies() {
            return _contentManager.Query<TaxonomyPart, TaxonomyPartRecord>().WithQueryHints(new QueryHints().ExpandParts<TitlePart>()).List();
        }

        public TaxonomyPart GetTaxonomy(int id) {
            return _contentManager.Get(id, VersionOptions.Published, new QueryHints().ExpandParts<TaxonomyPart, TitlePart>()).As<TaxonomyPart>();
        }

        public TaxonomyPart GetTaxonomyByName(string name) {
            if (String.IsNullOrWhiteSpace(name)) {
                throw new ArgumentNullException("name");
            }

            return _contentManager
                .Query<TaxonomyPart>()
                .Join<TitlePartRecord>()
                .Where(r => r.Title == name)
                .WithQueryHints(new QueryHints().ExpandRecords<CommonPartRecord>())
                .List()
                .FirstOrDefault();
        }

        public void CreateTermContentType(TaxonomyPart taxonomy) {
            // create the associated term's content type
            taxonomy.TermTypeName = GenerateTermTypeName(taxonomy.Name);

            _contentDefinitionManager.AlterTypeDefinition(taxonomy.TermTypeName, 
                cfg => cfg
                    .WithSetting("Taxonomy", taxonomy.Name)
                    .WithPart("TermPart")
                    .WithPart("TitlePart")
                    .WithPart("AutoroutePart", builder => builder
                        .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                        .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                        .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Taxonomy and Title', Pattern: '{Content.Container.Path}/{Content.Slug}', Description: 'my-taxonomy/my-term/sub-term'}]")
                        .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                    .WithPart("CommonPart")
                    .DisplayedAs(taxonomy.Name + " Term")
                );

        }

        public void DeleteTaxonomy(TaxonomyPart taxonomy) {
            _contentManager.Remove(taxonomy.ContentItem);

            // Removing terms
            foreach (var term in GetTerms(taxonomy.Id)) {
                DeleteTerm(term);
            }

            _contentDefinitionManager.DeleteTypeDefinition(taxonomy.TermTypeName);
        }

        public string GenerateTermTypeName(string taxonomyName) {
            var name = taxonomyName.ToSafeName() + "Term";
            int i = 2;
            while (_contentDefinitionManager.GetTypeDefinition(name) != null) {
                name = taxonomyName.ToSafeName() + i++;
            }

            return name;
        }

        public TermPart NewTerm(TaxonomyPart taxonomy) {
            var term = _contentManager.New<TermPart>(taxonomy.TermTypeName);
            term.TaxonomyId = taxonomy.Id;

            return term;
        }

        public IEnumerable<TermPart> GetTerms(int taxonomyId) {
            var result = _contentManager.Query<TermPart, TermPartRecord>()
                .Where(x => x.TaxonomyId == taxonomyId)
                .WithQueryHints(new QueryHints().ExpandRecords<TitlePartRecord, CommonPartRecord>())
                .List();

            return TermPart.Sort(result);
        }

        public IEnumerable<TermPart> GetAllTerms() {
            var result = _contentManager
                .Query<TermPart, TermPartRecord>()
                .WithQueryHints(new QueryHints().ExpandRecords<TitlePartRecord, CommonPartRecord>())
                .List();
            return TermPart.Sort(result);
        }

        public TermPart GetTerm(int id) {
            return _contentManager
                .Query<TermPart, TermPartRecord>()
                .WithQueryHints(new QueryHints().ExpandRecords<TitlePartRecord, CommonPartRecord>())
                .Where(x => x.Id == id).List().FirstOrDefault();
        }

        public IEnumerable<TermPart> GetTermsForContentItem(int contentItemId, string field = null) {
            return String.IsNullOrEmpty(field) 
                ? _termContentItemRepository.Fetch(x => x.TermsPartRecord.ContentItemRecord.Id == contentItemId).Select(t => GetTerm(t.TermRecord.Id))
                : _termContentItemRepository.Fetch(x => x.TermsPartRecord.Id == contentItemId && x.Field == field).Select(t => GetTerm(t.TermRecord.Id));
        }

        public TermPart GetTermByName(int taxonomyId, string name) {
            return _contentManager
                .Query<TermPart, TermPartRecord>()
                .WithQueryHints(new QueryHints().ExpandRecords<TitlePartRecord, CommonPartRecord>())
                .Where(t => t.TaxonomyId == taxonomyId)
                .Join<TitlePartRecord>()
                .Where(r => r.Title == name)
                .List()
                .FirstOrDefault();
        }

        public void CreateTerm(TermPart termPart) {
            if (GetTermByName(termPart.TaxonomyId, termPart.Name) == null) {
                _authorizationService.CheckAccess(Permissions.CreateTerm, _services.WorkContext.CurrentUser, null);

                termPart.As<ICommonPart>().Container = GetTaxonomy(termPart.TaxonomyId).ContentItem;
                _contentManager.Create(termPart);
            }
            else {
                _notifier.Warning(T("The term {0} already exists in this taxonomy", termPart.Name));
            }
        }

        public void DeleteTerm(TermPart termPart) {
            _contentManager.Remove(termPart.ContentItem);

            // delete termContentItems
            var termContentItems = _termContentItemRepository
                .Fetch(t => t.TermRecord == termPart.Record)
                .ToList();

            foreach (var termContentItem in termContentItems) {
                _termContentItemRepository.Delete(termContentItem);
            }
        }

        public void UpdateTerms(ContentItem contentItem, IEnumerable<TermPart> terms, string field) {
            var termsPart = contentItem.As<TermsPart>();

            // removing current terms for specific field
            var fieldIndexes = termsPart.Terms
                .Where(t => t.Field == field)
                .Select((t, i) => i)
                .OrderByDescending(i => i)
                .ToList();
            
            foreach(var x in fieldIndexes) {
                termsPart.Terms.RemoveAt(x);
            }
            
            // adding new terms list
            foreach(var term in terms) {
                termsPart.Terms.Add( 
                    new TermContentItem {
                        TermsPartRecord = termsPart.Record, 
                        TermRecord = term.Record, Field = field
                    });
            }
        }

        public IContentQuery<TermsPart, TermsPartRecord> GetContentItemsQuery(TermPart term, string fieldName = null) {

            var query = _contentManager
                .Query<TermsPart, TermsPartRecord>()
                .WithQueryHints(new QueryHints().ExpandRecords<TitlePartRecord, CommonPartRecord>());

            if (String.IsNullOrWhiteSpace(fieldName)) {
                query = query.Where(
                    tpr => tpr.Terms.Any(tr =>
                        tr.TermRecord.Id == term.Id));
            }
            else {
                query = query.Where(
                    tpr => tpr.Terms.Any(tr =>
                        tr.Field == fieldName
                         && (tr.TermRecord.Id == term.Id)));
            }

            return query;
        }
        
        public IEnumerable<IContent> GetContentItems(TermPart term, int skip = 0, int count = 0, string fieldName = null) {
            return GetContentItemsQuery(term, fieldName)
                .Join<CommonPartRecord>()
                .OrderByDescending(x => x.CreatedUtc)
                .Slice(skip, count);
        }

        public void MoveTerm(TaxonomyPart taxonomy, TermPart term, TermPart parentTerm) {
            
        }
    }
}
