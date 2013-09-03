using System.Linq;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;
using Coevery.Core.Models;
using Orchard.Projections.Models;

namespace Coevery.Core.Services {
    public class ViewPartService : IViewPartService {
        private readonly IRepository<ViewPartRecord> _viewPartRepository;
        private readonly IRepository<ProjectionPartRecord> _contentManager;
        private readonly IRepository<ContentTypeDefinitionRecord> _contentDefinitionManager;

        public ViewPartService(
            IRepository<ProjectionPartRecord> contentManager,
            IRepository<ViewPartRecord> viewPartRepository,
            IRepository<ContentTypeDefinitionRecord> contentDefinitionManager) {
            _viewPartRepository = viewPartRepository;
            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
        }

        public int GetProjectionId(string entityType) {
            var typeDefinitionRecord = _contentDefinitionManager.Get(t => t.Name == entityType);
            var viewPartRecord = _viewPartRepository.Fetch(t => t.ContentTypeDefinitionRecord_id == typeDefinitionRecord.Id).FirstOrDefault();

            if (viewPartRecord != null) {
                return viewPartRecord.ProjectionPartRecord_id;
            }
            return -1;
        }

        public void SetView(string entityType, int projectionId) {
            var typeDefinitionRecord = _contentDefinitionManager.Get(t => t.Name == entityType);
            var projectionPartRecord = _contentManager.Get(t => t.Id == projectionId);
            ViewPartRecord viewPartRecord = _viewPartRepository.Get(t => t.ContentTypeDefinitionRecord_id == typeDefinitionRecord.Id);
            if (viewPartRecord == null) {
                viewPartRecord = new ViewPartRecord {
                    ContentTypeDefinitionRecord_id = typeDefinitionRecord.Id,
                    ProjectionPartRecord_id = projectionPartRecord.Id
                };
                _viewPartRepository.Create(viewPartRecord);
            }
            else {
                viewPartRecord.ProjectionPartRecord_id = projectionPartRecord.Id;
            }
        }
    }
}