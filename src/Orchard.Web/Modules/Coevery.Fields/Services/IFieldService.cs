using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coevery.Fields.ViewModels;
using Orchard;
using Orchard.ContentManagement;

namespace Coevery.Fields.Services {
    public interface IFieldService : IDependency {
        void Create(string entityName, AddFieldViewModel viewModel, IUpdateModel updateModel);
    }
}
