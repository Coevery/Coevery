using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.CulturePicker.Models;
using Orchard.Data.Migration;

namespace Orchard.CulturePicker {
    public class Migrations : DataMigrationImpl {

        public int Create() {
            ContentDefinitionManager.AlterPartDefinition(typeof (CulturePickerPart).Name, cfg => cfg
                                                                                                          .Attachable());
            return 1;
        }

        public int UpdateFrom1() {
            ContentDefinitionManager.AlterTypeDefinition("CulturePickerWidget", cfg => cfg
                .WithPart("CulturePickerPart")
                .WithPart("WidgetPart")
                .WithPart("CommonPart")
                .WithSetting("Stereotype", "Widget"));

            return 2;
        }
    }
}