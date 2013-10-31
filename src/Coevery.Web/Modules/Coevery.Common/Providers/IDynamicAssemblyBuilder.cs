using System;
using Coevery;

namespace Coevery.Common.Providers {
    public interface IDynamicAssemblyBuilder : IDependency {
        bool Build();
        Type GetFieldType(string fieldNameType);
    }
}