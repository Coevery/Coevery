using System.Collections.Generic;

namespace Coevery.Mvc.ModelBinders {
    public interface IModelBinderProvider : IDependency {
        IEnumerable<ModelBinderDescriptor> GetModelBinders();
    }
}