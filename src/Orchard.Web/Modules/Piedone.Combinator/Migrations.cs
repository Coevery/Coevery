using System;
using System.Data;
using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Piedone.Combinator.Models;
using Piedone.Combinator.Services;

namespace Piedone.Combinator.Migrations
{
    [OrchardFeature("Piedone.Combinator")]
    public class Migrations : DataMigrationImpl
    {
        private readonly ICacheFileService _cacheFileService;

        public Migrations(ICacheFileService cacheFileService)
        {
            _cacheFileService = cacheFileService;
        }

        public int Create()
        {
            SchemaBuilder.CreateTable(typeof(CombinedFileRecord).Name,
                table => table
                    .Column<int>("Id", column => column.PrimaryKey().Identity())
                    .Column<int>("HashCode", column => column.NotNull())
                    .Column<int>("Slice")
                    .Column<string>("Type")
                    .Column<DateTime>("LastUpdatedUtc")
                    .Column<string>("Settings", column => column.Unlimited())
            ).AlterTable(typeof(CombinedFileRecord).Name,
                table => table
                    .CreateIndex("File", new string[] { "HashCode" })
            );

            SchemaBuilder.CreateTable(typeof(CombinatorSettingsPartRecord).Name,
                table => table
                    .ContentPartRecord()
                    .Column<string>("CombinationExcludeRegex", column => column.Unlimited())
                    .Column<bool>("CombineCdnResources")
                    .Column<bool>("MinifyResources")
                    .Column<string>("MinificationExcludeRegex", column => column.Unlimited())
                    .Column<bool>("EmbedCssImages")
                    .Column<int>("EmbeddedImagesMaxSizeKB")
                    .Column<string>("EmbedCssImagesStylesheetExcludeRegex", column => column.Unlimited())
                    .Column<string>("ResourceSetRegexes", column => column.Unlimited())
                    .Column<bool>("EnableForAdmin")
            );


            return 8;
        }

        public int UpdateFrom1()
        {
            SchemaBuilder.AlterTable(typeof(CombinatorSettingsPartRecord).Name,
                table =>
                {
                    table.AddColumn<bool>("MinifyResources");
                    table.AddColumn<string>("MinificationExcludeRegex");
                }
            );


            return 2;
        }

        public int UpdateFrom2()
        {
            SchemaBuilder.AlterTable(typeof(CombinatorSettingsPartRecord).Name,
                table => table
                    .AddColumn<string>("CombinationExcludeRegex")
            );

            SchemaBuilder.AlterTable(typeof(CombinedFileRecord).Name,
                table => table
                    .AddColumn<string>("Settings", column => column.Unlimited())
            );

            return 3;
        }

        public int UpdateFrom3()
        {
            SchemaBuilder.AlterTable(typeof(CombinatorSettingsPartRecord).Name,
                table =>
                {
                    table.AddColumn<bool>("EmbedCssImages");
                    table.AddColumn<int>("EmbeddedImagesMaxSizeKB");
                    table.AddColumn<string>("EmbedCssImagesStylesheetExcludeRegex");
                }
            );


            return 4;
        }

        public int UpdateFrom4()
        {
            SchemaBuilder.AlterTable(typeof(CombinatorSettingsPartRecord).Name,
                table => table
                    .AddColumn<string>("ResourceSetRegexes")
            );

            return 5;
        }

        public int UpdateFrom5()
        {
            SchemaBuilder.AlterTable(typeof(CombinatorSettingsPartRecord).Name,
                table => table
                    .AddColumn<bool>("EnableForAdmin")
            );

            return 6;
        }

        public int UpdateFrom6()
        {
            SchemaBuilder.AlterTable(typeof(CombinatorSettingsPartRecord).Name,
                table =>
                {
                    table.AlterColumn("CombinationExcludeRegex", column => column.WithType(DbType.String).Unlimited());
                    table.AlterColumn("MinificationExcludeRegex", column => column.WithType(DbType.String).Unlimited());
                    table.AlterColumn("EmbedCssImagesStylesheetExcludeRegex", column => column.WithType(DbType.String).Unlimited());
                    table.AlterColumn("ResourceSetRegexes", column => column.WithType(DbType.String).Unlimited());
                }
            );


            return 7;
        }

        public int UpdateFrom7()
        {
            _cacheFileService.Empty();

            SchemaBuilder.AlterTable(typeof(CombinatorSettingsPartRecord).Name,
                table =>
                {
                    table.DropColumn("CombineCDNResources");
                    table.AddColumn<bool>("CombineCdnResources");
                }
            );

            return 8;
        }


        public void Uninstall()
        {
            _cacheFileService.Empty();
        }
    }
}