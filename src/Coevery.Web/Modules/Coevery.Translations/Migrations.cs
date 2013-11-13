using Coevery.Data.Migration;
using Coevery.Translations.Models;

namespace Coevery.Translations
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable(
                typeof(LocalizableStringRecord).Name,
                table =>
                table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Path")
                    .Column<string>("Context")
                    .Column<string>("StringKey", column => column.WithLength(4000))
                    .Column<string>("OriginalLanguageString",
                                    column => column.WithLength(4000))
                );
            SchemaBuilder.CreateTable(
                typeof(TranslationRecord).Name,
                table =>
                table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<string>("Culture")
                    .Column<string>("Value", column => column.WithLength(4000))
                    .Column<int>("LocalizableStringRecord_Id")
                )
                //ForeignKey names must be unique, so enforce this using part of GUID
                //Goes wrong when multitenancy is used with table prefixes in same database
                .CreateForeignKey(
                    string.Format("FK_Po_Translation_LocalizableString_{0}", System.Guid.NewGuid().ToString("N").Substring(0, 16).ToUpper()),
                    "Coevery.Translations", typeof(TranslationRecord).Name,
                    new[] { "LocalizableStringRecord_Id" },
                    "Coevery.Translations", typeof(LocalizableStringRecord).Name,
                    new[] { "Id" });

            SchemaBuilder.AlterTable(
                typeof(TranslationRecord).Name,
                table => table.CreateIndex(
                    "Index_Po_Translation_LocalizableStringRecord_Id",
                    "LocalizableStringRecord_Id"));
            return 1;
        }

    }
}