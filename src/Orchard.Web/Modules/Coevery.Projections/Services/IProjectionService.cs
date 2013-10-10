using System.Collections.Generic;
using Coevery.Projections.ViewModels;
using Orchard;
using Orchard.Projections.Descriptors.Property;

namespace Coevery.Projections.Services {
    public interface IProjectionService : IDependency {
        IEnumerable<PropertyDescriptor> GetFieldDescriptors(string entityType);
        ProjectionEditViewModel GetProjectionViewModel(int id);
        string UpdateViewOnEntityAltering(string entityName);
        int EditPost(int id, ProjectionEditViewModel viewModel, IEnumerable<string> pickedFields);
    }
}