using System.Linq;
using Coevery.Entities.Events;
using Coevery.Projections.Models;
using Coevery.Projections.Services;
using Coevery.Projections.ViewModels;
using Orchard.ContentManagement;

namespace Coevery.Projections.Handlers {
    public class ProjectionEntityEventsHandler : IEntityEvents {
        private readonly IProjectionService _projectionService;
        private readonly IContentManager _contentManager;

        public ProjectionEntityEventsHandler(
            IProjectionService projectionService,
            IContentManager contentManager) {
            _projectionService = projectionService;
            _contentManager = contentManager;
        }

        public void OnCreated(string entityName) {
            var fields = _projectionService.GetFieldDescriptors(entityName).Select(x => x.Type);
            var viewModel = new ProjectionEditViewModel {
                ItemContentType = entityName,
                DisplayName = entityName + " DefaultView",
                PageRowCount = 50,
                IsDefault = true
            };
            _projectionService.EditPost(0, viewModel, fields);
        }

        public void OnUpdating(string entityName) {
            _projectionService.UpdateViewOnEntityAltering(entityName);
        }

        public void OnDeleting(string entityName) {
            var projections = _contentManager.Query<ListViewPart, ListViewPartRecord>().Where(x => x.ItemContentType == entityName).List();
            foreach (var projection in projections) {
                _contentManager.Remove(projection.ContentItem);
            }
        }
    }
}