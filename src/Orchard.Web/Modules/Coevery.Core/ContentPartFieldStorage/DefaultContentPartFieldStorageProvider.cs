using Orchard.ContentManagement;
using Orchard.ContentManagement.FieldStorage;
using Orchard.ContentManagement.MetaData.Models;

namespace Coevery.Core.ContentPartFieldStorage {
    public class DefaultContentPartFieldStorageProvider : IFieldStorageProvider {

        public string ProviderName {
            get { return "Part"; }
        }

        public IFieldStorage BindStorage(ContentPart contentPart, ContentPartFieldDefinition partFieldDefinition) {
            return new DefaultCotentPartFieldStorage(contentPart, partFieldDefinition);
        }
    }
}
