using System.Collections.Generic;
using Coevery.Core.Common.ViewModels;
using Coevery.Projections.ViewModels;
using Coevery;
using Coevery.Projections.Descriptors.Property;

namespace Coevery.Projections.Services {
    public interface IProjectionService : IDependency {
        IEnumerable<PicklistItemViewModel> GetFieldDescriptors(string entityType, int projectionId);
        ProjectionEditViewModel GetProjectionViewModel(int id);
        string UpdateViewOnEntityAltering(string entityName);
        int EditPost(int id, ProjectionEditViewModel viewModel, IEnumerable<string> pickedFields);
    }
}