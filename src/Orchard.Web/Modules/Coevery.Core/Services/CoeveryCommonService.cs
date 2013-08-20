using Coevery.Core.Models.Common;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;

namespace Coevery.Core.Services {
    public class CoeveryCommonService : ICoeveryCommonService {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;

        public CoeveryCommonService(IContentDefinitionManager contentDefinitionManager,
                             IContentManager contentManager) {

            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
        }

        public void WeldCommonPart(string typeName) {
            var builder = new ContentItemBuilder(_contentDefinitionManager.GetTypeDefinition(typeName));
            builder.Weld<ContentPart<CoeveryCommonPartVersionRecord>>();
            _contentDefinitionManager.AlterTypeDefinition(typeName, cfg => cfg.WithPart("CoeveryCommonPart"));
        }
    }
}