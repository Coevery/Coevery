using System;
using Coevery.ContentManagement.MetaData.Models;

namespace Coevery.ContentManagement.MetaData {
    public class ContentPartInfo {
        public string PartName { get; set; }
        public Func<ContentTypePartDefinition, ContentPart> Factory { get; set; }
    }
}
