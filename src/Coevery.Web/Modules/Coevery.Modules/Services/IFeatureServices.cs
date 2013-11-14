using System.Collections.Generic;
using Coevery.Modules.Models;

namespace Coevery.Modules.Services {
    public interface IFeatureServices : IDependency
    {
        IEnumerable<FeatureCategory> GeFeatureCategories(FeaturesBulkAction bulkAction, IList<string> featureIds);
    }
}