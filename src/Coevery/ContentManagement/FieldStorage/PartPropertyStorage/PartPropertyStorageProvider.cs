using Coevery.ContentManagement.MetaData.Models;

namespace Coevery.ContentManagement.FieldStorage.PartPropertyStorage {
    public class PartPropertyStorageProvider : IFieldStorageProvider {

        public string ProviderName {
            get { return "Part"; }
        }

        public IFieldStorage BindStorage(ContentPart contentPart, ContentPartFieldDefinition partFieldDefinition) {
            return new PartPropertyFieldStorage(contentPart, partFieldDefinition);
        }
    }
}
