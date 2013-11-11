using System.Collections.Generic;
using Coevery.Core.Common.ViewModels;
using Coevery.Projections.Descriptors.Property;

namespace Coevery.Projections.ViewModels
{
    public class ProjectionEditViewModel {
        public ProjectionEditViewModel() {
            Fields = new List<PicklistItemViewModel>();
        }

        public int Id { get; set; }
        public string ItemContentType { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<PicklistItemViewModel> Fields { get; set; }
        public IEnumerable<string> PickedFields { get; set; }
        public string VisableTo { get; set; }
        public int PageRowCount { get; set; }
        public string SortedBy { get; set; }
        public string SortMode { get; set; }
        public bool IsDefault { get; set; }
    }
}