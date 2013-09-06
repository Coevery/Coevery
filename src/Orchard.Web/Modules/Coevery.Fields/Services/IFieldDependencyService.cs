using System;
using System.Collections.Generic;
using Orchard;

namespace Coevery.Fields.Services {
    public interface IFieldDependencyService : IDependency {
        object Get(string entityName);
        bool Delete(int id);
        bool Create(string entityName, string controlFieldName, string dependentFieldName, DependencyValuePair[] mappingValue);
        string GetDependencyMap(int dependentId);
        bool Edit(int id, string newDependency);
    }
}