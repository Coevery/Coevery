using System;
using System.Collections.Generic;
using Coevery.Dynamic;
using Orchard;

namespace Coevery.Metadata.Services
{
    public interface ITableSchemaManager : IDependency {
        void UpdateSchema(Func<string, string> format,
            IEnumerable<Type> types );
    }
}