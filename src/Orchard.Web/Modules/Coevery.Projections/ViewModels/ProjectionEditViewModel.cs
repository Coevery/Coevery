using System.Collections.Generic;
using Orchard.Projections.Descriptors.Property;

namespace Coevery.Projections.ViewModels
{
    public class ProjectionEditViewModel {
        public ProjectionEditViewModel() {
            Fields = new List<PropertyDescriptor>();
        }

        public int Id { get; set; }
        public string ItemContentType { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<PropertyDescriptor> Fields { get; set; }
        public string VisableTo { get; set; }
        public int PageRowCount { get; set; }
        public string SortedBy { get; set; }
        public string SortMode { get; set; }
        public bool IsDefault { get; set; }
    }
}