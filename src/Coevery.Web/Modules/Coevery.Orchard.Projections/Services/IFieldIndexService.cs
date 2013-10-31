using System;
using Coevery.Orchard.Projections.Models;

namespace Coevery.Orchard.Projections.Services {
    public interface IFieldIndexService : IDependency {
        void Set(FieldIndexPart part, string partName, string fieldName, string valueName, object value, Type valueType);
        T Get<T>(FieldIndexPart part, string partName, string fieldName, string valueName);
    }
}
