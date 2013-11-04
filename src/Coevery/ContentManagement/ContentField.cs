using Coevery.ContentManagement.FieldStorage;
using Coevery.ContentManagement.MetaData.Models;

namespace Coevery.ContentManagement {
    public class ContentField {
        public string Name { get { return PartFieldDefinition.Name; } }
        public string DisplayName { get { return PartFieldDefinition.DisplayName; } }

        public ContentPartFieldDefinition PartFieldDefinition { get; set; }
        public ContentFieldDefinition FieldDefinition { get { return PartFieldDefinition.FieldDefinition; } }

        public IFieldStorage Storage { get; set; }
    }
}
