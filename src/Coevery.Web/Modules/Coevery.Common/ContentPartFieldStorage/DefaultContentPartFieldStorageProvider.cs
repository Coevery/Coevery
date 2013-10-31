using Coevery.ContentManagement;
using Coevery.ContentManagement.FieldStorage;
using Coevery.ContentManagement.MetaData.Models;

namespace Coevery.Common.ContentPartFieldStorage {
    public class DefaultContentPartFieldStorageProvider : IFieldStorageProvider {

        public string ProviderName {
            get { return "Part"; }
        }

        public IFieldStorage BindStorage(ContentPart contentPart, ContentPartFieldDefinition partFieldDefinition) {
            return new DefaultCotentPartFieldStorage(contentPart, partFieldDefinition);
        }
    }
}
