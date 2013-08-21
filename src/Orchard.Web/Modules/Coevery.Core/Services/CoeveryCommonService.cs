using Coevery.Core.Models.Common;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using System.Linq;
using Orchard.Data;

namespace Coevery.Core.Services {
    public class CoeveryCommonService : ICoeveryCommonService {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IRepository<CoeveryCommonPartRecord> _commonPartRepository;
        private readonly IRepository<CoeveryCommonPartVersionRecord> _commonPartVersionRepository;
        private readonly IContentManager _contentManager;

        public CoeveryCommonService(
            IContentDefinitionManager contentDefinitionManager,
            IRepository<CoeveryCommonPartRecord> commonPartRepository,
            IRepository<CoeveryCommonPartVersionRecord> commonPartVersionRepository,
            IContentManager contentManager) {
            _commonPartRepository = commonPartRepository;
            _commonPartVersionRepository = commonPartVersionRepository;
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
        }

        public void WeldCommonPart(string typeName) {            
            //contentType.Weld(new CoeveryCommonPart());
            var contentType = _contentManager.New(typeName);
            //var commonPart = _contentManager.New<CoeveryCommonPart>(typeName);
            //commonPart.Container = contentType;

            //_contentManager.Create(commonPart,VersionOptions.Published);
        }
    }
}