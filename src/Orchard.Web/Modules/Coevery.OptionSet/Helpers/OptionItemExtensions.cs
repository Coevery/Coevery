using Coevery.OptionSet.Models;
using Coevery.OptionSet.ViewModels;

namespace Coevery.OptionSet.Helpers {
    public static class OptionItemExtensions {
        public static OptionItemEntry CreateTermEntry(this OptionItemPart term) {
            return new OptionItemEntry {
                Id = term.Id,
                Name = term.Name,
                Selectable = term.Selectable,
                Weight= term.Weight,
                IsChecked = false,
                ContentItem = term.ContentItem,
                OptionSetId = term.OptionSetId
            };
        }
    }
}