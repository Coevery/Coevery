using System.Collections.Generic;
using Coevery.OptionSet.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Coevery.OptionSet.Services {
    public interface IOptionSetService : IDependency {
        IEnumerable<OptionSetPart> GetTaxonomies();
        OptionSetPart GetTaxonomy(int id);
        OptionSetPart GetTaxonomyByName(string name);
        void DeleteTaxonomy(OptionSetPart taxonomy);

        IEnumerable<OptionItemPart> GetTerms(int optionSetId);
        OptionItemPart GetOptionItem(int id);
        OptionItemPart GetTermByName(int taxonomyId, string name);
        void DeleteTerm(OptionItemPart termPart);

        string GenerateTermTypeName(string taxonomyName);
        OptionItemPart NewTerm(OptionSetPart taxonomy);
        IEnumerable<OptionItemPart> GetOptionItemsForContentItem(int contentItemId, string field = null);
        void UpdateTerms(ContentItem contentItem, IEnumerable<OptionItemPart> optionItems, string field);
        IEnumerable<IContent> GetContentItems(OptionItemPart term, int skip = 0, int count = 0, string fieldName = null);
    }
}