using System;
using System.Collections.Generic;
using Coevery.Dynamic;
using Orchard;

namespace Coevery.Metadata.Services
{
    public interface ITableSchemaManager : IDependency {
        void UpdateSchema(IEnumerable<DynamicTypeDefinition> typeDefinitions, 
            Func<string, string> format,
            IEnumerable<Type> types );
    }
}