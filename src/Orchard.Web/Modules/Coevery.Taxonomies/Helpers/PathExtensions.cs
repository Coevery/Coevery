using Coevery.Taxonomies.Models;
using System;
using Coevery.Taxonomies.ViewModels;
using System.Linq;

namespace Coevery.Taxonomies.Helpers {
    public static class TermExtensions {
        public static TermEntry CreateTermEntry(this TermPart term) {
            return new TermEntry {
                Id = term.Id,
                Name = term.Name,
                Selectable = term.Selectable,
                Weight= term.Weight,
                IsChecked = false,
                ContentItem = term.ContentItem
            };
        }
    }
}