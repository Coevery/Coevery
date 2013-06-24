using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Projections.ViewModels;
using Orchard;
using Orchard.Projections.Models;
using Orchard.Projections.ViewModels;


namespace Coevery.Projections.Services
{
    public interface IProjectionService : IDependency
    {
        ProjectionEditViewModel CreateTempProjection(string entityType);
        AdminEditViewModel GetQueryViewModel(QueryPart query);
        ProjectionEditViewModel GetProjectionViewModel(int id);
        bool EditPost(int id, ProjectionEditViewModel viewModel, IEnumerable<string> pickedFields);
    }
}