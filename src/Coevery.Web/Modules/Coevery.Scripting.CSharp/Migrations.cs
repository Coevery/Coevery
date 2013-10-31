using Coevery.ContentManagement.MetaData;
using Coevery.Core.Contents.Extensions;
using Coevery.Data.Migration;
using Coevery.Environment.Extensions;

namespace Coevery.Scripting.CSharp {
    [CoeveryFeature("Coevery.Scripting.CSharp.Validation")]
    public class Migrations : DataMigrationImpl {

        public int Create() {
            
            ContentDefinitionManager.AlterPartDefinition("ScriptValidationPart", cfg => cfg
                .Attachable()
                .WithDescription("Provides a way to validate content items using C#.")
                );

            return 1;
        }
    }
}