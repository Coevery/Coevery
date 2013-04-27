using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.Projections.Descriptors.Property;
using Orchard.Projections.ViewModels;

namespace Coevery.Metadata.ViewModels
{
    public class ProjectionEditViewModel
    {
        public ProjectionEditViewModel()
        {
            QueryViewModel = new AdminEditViewModel();
            LayoutViewModel = new LayoutEditViewModel();
            AllFields = new List<PropertyDescriptor>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }

        public AdminEditViewModel QueryViewModel { get; set; }
        public LayoutEditViewModel LayoutViewModel { get; set; }
        public IEnumerable<PropertyDescriptor> AllFields { get; set; }
        public string VisableTo { get; set; }

    }
}