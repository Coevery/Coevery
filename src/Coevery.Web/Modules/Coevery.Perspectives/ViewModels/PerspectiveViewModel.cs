using System.Collections.Generic;
using Coevery.ContentManagement.Handlers;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.Core.Common.ViewModels;

namespace Coevery.Perspectives.ViewModels {
    public class PerspectiveViewModel {
        public static readonly IEnumerable<string> NavigationTypes = new[] {
            "ModuleMenuItem", "MenuItem"
        };

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Position { get; set; }
        public IList<ContentTypeDefinition> NavigationTypeList { get; set; }
    }
}