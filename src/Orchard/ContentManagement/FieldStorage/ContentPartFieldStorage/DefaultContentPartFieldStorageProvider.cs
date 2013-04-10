namespace Orchard.ContentManagement.FieldStorage.ContentPartFieldStorage {
    public class DefaultContentPartFieldStorageProvider : IFieldStorageProvider {
        public string ProviderName {
            get { return "Part"; }
        }

        public IFieldStorage BindStorage(ContentPart contentPart, MetaData.Models.ContentPartFieldDefinition partFieldDefinition) {
            return new DefaultCotentPartFieldStorage(contentPart);
        }
    }
}
