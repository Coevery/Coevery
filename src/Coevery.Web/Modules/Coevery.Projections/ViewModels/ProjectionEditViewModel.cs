using System.Collections.Generic;
using Coevery.Core.Common.ViewModels;
using Coevery.Projections.Descriptors.Layout;

namespace Coevery.Projections.ViewModels
{
    public class ProjectionEditViewModel {
        public ProjectionEditViewModel() {
            Fields = new List<PicklistItemViewModel>();
            State = new Dictionary<string, string>();
        }

        public int Id { get; set; }
        public string ItemContentType { get; set; }
        public string DisplayName { get; set; }
        public IEnumerable<PicklistItemViewModel> Fields { get; set; }
        public IEnumerable<string> PickedFields { get; set; }
        public string VisableTo { get; set; }
        public bool IsDefault { get; set; }

        public int LayoutId { get; set; }
        public LayoutDescriptor Layout { get; set; }
        public dynamic Form { get; set; }
        public IDictionary<string,string> State { get; set; }
    }
}