using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Models;

namespace Coevery.Tests.ContentManagement.Models {
    public class Phi : ContentField {
        public Phi() {
            PartFieldDefinition = new ContentPartFieldDefinition(new ContentFieldDefinition("Phi"), "Phi", new SettingsDictionary());
        }
    }
}
