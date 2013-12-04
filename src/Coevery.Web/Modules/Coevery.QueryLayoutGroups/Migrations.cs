using System.Collections.Generic;
using System.Data;
using System.Linq;
using Coevery.Core.Common.Models;
using Coevery.Core.Contents.Extensions;
using Coevery.Core.Title.Models;
using Coevery.Localization;
using Coevery.ContentManagement.MetaData;
using Coevery.Data;
using Coevery.Data.Migration;
using Coevery.Environment.Configuration;

namespace Coevery.QueryLayoutGroups {
    public class Migrations : DataMigrationImpl {

        public int Create() {
            // Properties index

            SchemaBuilder.CreateTable("LayoutGroupRecord",
                table => table
                    .ContentPartRecord()
                    .Column<int>("QueryPartRecord_id")
                    .Column<int>("LayoutRecord_Id")
                );

            SchemaBuilder.CreateTable("MemberBindingRecord",
                table => table
                    .Column<int>("Id", c => c.PrimaryKey().Identity())
                    .Column<string>("Type", c => c.WithLength(255))
                    .Column<string>("Member", c => c.WithLength(64))
                    .Column<string>("Description", c => c.WithLength(500))
                    .Column<string>("DisplayName", c => c.WithLength(64))
                );

            ContentDefinitionManager.AlterTypeDefinition("ProjectionWidget",
                cfg => cfg
                    .WithPart("WidgetPart")
                    .WithPart("CommonPart")
                    .WithPart("IdentityPart")
                    .WithPart("ProjectionPart")
                    .WithSetting("Stereotype", "Widget")
                );

            ContentDefinitionManager.AlterTypeDefinition("ProjectionPage",
                cfg => cfg
                    .WithPart("CommonPart")
                    .WithPart("TitlePart")
                     .WithPart("AutoroutePart", builder => builder
                        .WithSetting("AutorouteSettings.AllowCustomPattern", "true")
                        .WithSetting("AutorouteSettings.AutomaticAdjustmentOnEdit", "false")
                        .WithSetting("AutorouteSettings.PatternDefinitions", "[{Name:'Title', Pattern: '{Content.Slug}', Description: 'my-projections'}]")
                        .WithSetting("AutorouteSettings.DefaultPatternIndex", "0"))
                    .WithPart("MenuPart")
                    .WithPart("ProjectionPart")
                    .WithPart("AdminMenuPart", p => p.WithSetting("AdminMenuPartTypeSettings.DefaultPosition", "5"))
                    .Creatable()
                    .DisplayedAs("Projection")
                );

            // Default Model Bindings - CommonPartRecord

            

            ContentDefinitionManager.AlterTypeDefinition("LayoutProperty", cfg => cfg
                .WithPart("LayoutPropertyPart")
                .DisplayedAs("LayoutProperty")
                );
            return 1;
        }

        
    }
}