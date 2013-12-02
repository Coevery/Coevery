using System.Linq;
using Coevery.Common.Extensions;
using Coevery.Entities.Events;
using Coevery.Projections.Models;
using Coevery.Projections.Services;
using Coevery.Projections.ViewModels;
using Coevery.ContentManagement;

namespace Coevery.Projections.Handlers {
    public class ProjectionEntityEventsHandler : IEntityEvents {
        private readonly IProjectionService _projectionService;
        private readonly IContentManager _contentManager;
        private readonly IProjectionManager _projectionManager;

        public ProjectionEntityEventsHandler(
            IProjectionService projectionService,
            IProjectionManager projectionManager,
            IContentManager contentManager) {
            _projectionService = projectionService;
            _contentManager = contentManager;
            _projectionManager = projectionManager;
        }

        public void OnCreated(string entityName) {
            var fields = _projectionService.GetFieldDescriptors(entityName,-1).Select(x => x.Value);
            var viewModel = new ProjectionEditViewModel {
                ItemContentType = entityName.ToPartName(),
                DisplayName = entityName + " DefaultView",
                IsDefault = true,
                Layout = _projectionManager.DescribeLayouts()
                    .SelectMany(descr => descr.Descriptors)
                    .FirstOrDefault(descr => descr.Category == "Grids" && descr.Type == "Default")
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