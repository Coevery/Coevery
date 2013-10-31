using System.Collections.Generic;

namespace Coevery.Mvc.ModelBinders {
    public interface IModelBinderPublisher : IDependency {
        void Publish(IEnumerable<ModelBinderDescriptor> binders);
    }
}