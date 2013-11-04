using Coevery.Data.Migration.Schema;

namespace Coevery.Data.Migration.Interpreters {
    public interface IDataMigrationInterpreter : IDependency{
        void Visit(ISchemaBuilderCommand command);
        void Visit(CreateTableCommand command);
        void Visit(DropTableCommand command);
        void Visit(AlterTableCommand command);
        void Visit(SqlStatementCommand command);
        void Visit(CreateForeignKeyCommand command);
        void Visit(DropForeignKeyCommand command);
    }
}
