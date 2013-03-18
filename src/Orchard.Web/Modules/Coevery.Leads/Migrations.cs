using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Coevery.Leads
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("TodoListRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("Title")
                    .Column<string>("UserId")
                );

            SchemaBuilder.CreateTable("TodoItemRecord",
                table => table
                    .ContentPartRecord()
                    .Column<string>("Title")
                    .Column<bool>("IsDone")
                    .Column<int>("ToDoList_id")
                );

            SchemaBuilder.CreateTable("LeadRecord",
               table => table
                   .ContentPartRecord()
                   .Column<string>("Topic")
                   .Column<string>("FirstName")
                   .Column<string>("LastName")
                   .Column<int>("StatusCode")
               );

            ContentDefinitionManager.AlterPartDefinition("Lead",
                cfg => cfg
                    .WithField("Topic", builder => builder
                        .WithDisplayName("Topic")
                        .OfType("TextField")
                        .WithSetting("TextFieldSettings.Required", "True"))
                    .WithField("FirstName", builder => builder
                        .WithDisplayName("FirstName")
                        .OfType("TextField")
                        .WithSetting("TextFieldSettings.Required", "True"))
                    .WithField("LastName", builder => builder
                        .WithDisplayName("LastName")
                        .OfType("TextField")
                        .WithSetting("TextFieldSettings.Required", "True"))
                    .WithField("StatusCode", builder => builder
                        .WithDisplayName("Status")
                        .OfType("TextField"))
                    .Attachable()
                );

            ContentDefinitionManager.AlterTypeDefinition("Lead",
               cfg => cfg
                   .WithPart("Lead")
                   .WithPart("LocalizationPart")
                   .Creatable()
               );

            return 1;
        }
    }
}