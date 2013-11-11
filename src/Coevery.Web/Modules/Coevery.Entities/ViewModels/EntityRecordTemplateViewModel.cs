using Coevery.ContentManagement.ViewModels;

namespace Coevery.Entities.ViewModels {
    public class EntityRecordViewModel {
        public string FieldTypeName { get; set; }
        public string FieldTypeDisplayName { get; set; }
        public TemplateViewModel TemplateViewModel { get; set; }
    }
}