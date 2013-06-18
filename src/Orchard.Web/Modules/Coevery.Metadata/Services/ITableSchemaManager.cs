using System;
using System.Collections.Generic;
using Orchard;
using Orchard.Environment.ShellBuilders.Models;

namespace Coevery.Metadata.Services
{
    public interface ITableSchemaManager : IDependency {
        void UpdateSchema(IEnumerable<RecordBlueprint> recordBlueprints);
    }
}