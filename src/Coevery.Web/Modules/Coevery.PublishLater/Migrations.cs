using Coevery.ContentManagement.MetaData;
using Coevery.Core.Contents.Extensions;
using Coevery.Data.Migration;

namespace Coevery.PublishLater {
    public class Migrations : DataMigrationImpl {
        public int Create() {
            ContentDefinitionManager.AlterPartDefinition("PublishLaterPart", builder => builder.Attachable());

            return 1;
        }

        public int UpdateFrom1() {
            ContentDefinitionManager.AlterPartDefinition("PublishLaterPart", builder => builder
                .WithDescription("Adds the ability to delay the publication of a content item to a later date and time."));

            return 2;
        }
    }
}