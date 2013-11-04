using Coevery.ContentManagement.MetaData.Builders;

namespace Coevery.Indexing {
    public static class MetaDataExtensions {
        public static ContentTypeDefinitionBuilder Indexed(this ContentTypeDefinitionBuilder builder, params string[] indexes) {
            return builder.WithSetting("TypeIndexing.Indexes", string.Join(",", indexes ?? new string[0]));
        }
    }
}
