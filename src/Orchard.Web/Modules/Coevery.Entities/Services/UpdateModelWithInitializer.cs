using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;

namespace Coevery.Entities.Services {
    public interface IModelInitializer {
        void InitializeModel<TModel>(TModel model) where TModel : class;
    }
}