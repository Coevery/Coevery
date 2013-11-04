using Coevery.Environment.Extensions.Models;

namespace Coevery.Data.Migration {
    public interface IDataMigration : IDependency {
        Feature Feature { get; }
    }
}
