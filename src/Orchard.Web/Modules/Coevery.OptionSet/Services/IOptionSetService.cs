using System.Collections.Generic;
using Coevery.OptionSet.Models;
using Coevery.OptionSet.ViewModels;
using Orchard;
using Orchard.ContentManagement;

namespace Coevery.OptionSet.Services {
    public interface IOptionSetService : IDependency {
        IEnumerable<OptionSetPart> GetOptionSets();
        OptionSetPart GetOptionSet(int id);
        OptionSetPart GetOptionSetByName(string name);
        void DeleteOptionSet(OptionSetPart taxonomy);

        IEnumerable<OptionItemPart> GetOptionItems(int optionSetId);
        OptionItemPart GetOptionItem(int id);
        OptionItemPart GetTermByName(int taxonomyId, string name);
        void DeleteOptionItem(OptionItemPart optionItemPart);

        string GenerateTermTypeName(string taxonomyName);
        OptionItemPart NewTerm(OptionSetPart taxonomy);
        bool CreateTerm(OptionItemPart termPart);
        bool EditOptionItem(OptionItemEntry newItem);
        IEnumerable<OptionItemPart> GetOptionItemsForContentItem(int contentItemId, string field = null);
        void UpdateTerms(ContentItem contentItem, IEnumerable<OptionItemPart> optionItems, string field);
        IEnumerable<IContent> GetContentItems(OptionItemPart term, int skip = 0, int count = 0, string fieldName = null);
    }
}