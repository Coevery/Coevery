using System.Collections.Generic;
using Coevery.ContentManagement.MetaData.Models;

namespace Coevery.Core.Contents.ViewModels {
    public class ListContentTypesViewModel  {
        public IEnumerable<ContentTypeDefinition> Types { get; set; }
    }
}