using System.Collections.Generic;
using Coevery.Projections.ViewModels;
using Orchard;
using Orchard.Projections.Models;
using Orchard.Projections.ViewModels;

namespace Coevery.Projections.Services {
    public interface IProjectionService : IDependency {
        int CreateProjection(string entityType);
        ProjectionEditViewModel GetTempProjection(string entityType);
        AdminEditViewModel GetQueryViewModel(QueryPart query);
        ProjectionEditViewModel GetProjectionViewModel(int id);
        void EditPost(int id, ProjectionEditViewModel viewModel, IEnumerable<string> pickedFields);
    }
}