using System;
using System.Collections.Generic;
using Orchard;
using Orchard.ContentManagement;
using Coevery.Taxonomies.Models;

namespace Coevery.Taxonomies.Services {
    public interface ITaxonomyService : IDependency {
        IEnumerable<TaxonomyPart> GetTaxonomies();
        TaxonomyPart GetTaxonomy(int id);
        TaxonomyPart GetTaxonomyByName(string name);
        void CreateTermContentType(TaxonomyPart taxonomy);
        void DeleteTaxonomy(TaxonomyPart taxonomy);

        IEnumerable<TermPart> GetAllTerms();
        IEnumerable<TermPart> GetTerms(int taxonomyId);
        TermPart GetTerm(int id);
        TermPart GetTermByName(int taxonomyId, string name);
        void DeleteTerm(TermPart termPart);
        void MoveTerm(TaxonomyPart taxonomy, TermPart term, TermPart parentTerm);

        string GenerateTermTypeName(string taxonomyName);
        TermPart NewTerm(TaxonomyPart taxonomy);
        IEnumerable<TermPart> GetTermsForContentItem(int contentItemId, string field = null);
        void UpdateTerms(ContentItem contentItem, IEnumerable<TermPart> terms, string field);
        IEnumerable<IContent> GetContentItems(TermPart term, int skip = 0, int count = 0, string fieldName = null);
        IContentQuery<TermsPart, TermsPartRecord> GetContentItemsQuery(TermPart term, string fieldName = null);

    }
}