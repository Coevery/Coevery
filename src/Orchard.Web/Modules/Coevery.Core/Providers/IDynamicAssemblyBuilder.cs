using System;
using Orchard;

namespace Coevery.Core.Providers {
    public interface IDynamicAssemblyBuilder : IDependency {
        bool Build();
        Type GetFieldType(string fieldNameType);
    }
}