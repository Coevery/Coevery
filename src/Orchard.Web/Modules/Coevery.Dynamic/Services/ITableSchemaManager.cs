using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard;

namespace Coevery.Dynamic.Services
{
    public interface ITableSchemaManager : IDependency {
        void UpdateSchema(IEnumerable<DynamicTypeDefinition> typeDefinitions, Func<string, string> format);
    }
}