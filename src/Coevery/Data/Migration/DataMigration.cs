using Coevery.ContentManagement.MetaData;
using Coevery.Data.Migration.Schema;
using Coevery.Environment.Extensions.Models;

namespace Coevery.Data.Migration {
    /// <summary>
    /// Data Migration classes can inherit from this class to get a SchemaBuilder instance configured with the current tenant database prefix
    /// </summary>
    public abstract class DataMigrationImpl : IDataMigration {
        public virtual SchemaBuilder SchemaBuilder { get; set; }
        public virtual IContentDefinitionManager ContentDefinitionManager { get; set; }
        public virtual Feature Feature { get; set; }
    }
}