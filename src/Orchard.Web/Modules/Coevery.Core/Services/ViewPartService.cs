using Orchard.ContentManagement;
using System.Linq;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Settings.Metadata.Records;
using Orchard.Data;
using Coevery.Core.Models;
using Orchard.Projections.Models;

namespace Coevery.Core.Services
{
    public class ViewPartService : IViewPartService
    {
        private readonly IRepository<ViewPartRecord> _viewPartRepository;
        private readonly IRepository<ProjectionPartRecord> _contentManager;
        private readonly IRepository<ContentTypeDefinitionRecord> _contentDefinitionManager;

        public ViewPartService(IRepository<ProjectionPartRecord> contentManager,
                               IRepository<ViewPartRecord> viewPartRepository,
                               IRepository<ContentTypeDefinitionRecord> contentDefinitionManager)
        {
            _viewPartRepository = viewPartRepository;
            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;
        }

        public int GetProjectionId(string entityType)
        {
            var viewPartRecord = _viewPartRepository.Fetch(t => t.ContentTypeDefinitionRecord.Name == entityType).FirstOrDefault();

            if (viewPartRecord != null)
            {
                return viewPartRecord.ProjectionPartRecord.Id;
            }
            else
            {
                return -1;
            }

        }

        public void SetView(string entityType, int projectionId)
        {
            var typeDefinitionRecord = _contentDefinitionManager.Get(t => t.Name == entityType);
            var projectionPartRecord = _contentManager.Get(t => t.Id == projectionId);
            ViewPartRecord viewPartRecord = _viewPartRepository.Get(t => t.ContentTypeDefinitionRecord.Name == entityType);
            if (viewPartRecord == null)
            {
                viewPartRecord = new ViewPartRecord
                {
                    ContentTypeDefinitionRecord = typeDefinitionRecord,
                    ProjectionPartRecord = projectionPartRecord
                };
                _viewPartRepository.Create(viewPartRecord);
            }
            else
            {
                viewPartRecord.ProjectionPartRecord = projectionPartRecord;
            }
           
        }
    }
}