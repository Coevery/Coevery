using System.Linq;
using Coevery.Core.Services;
using Coevery.Entities.Events;
using Coevery.Projections.Services;
using Coevery.Projections.ViewModels;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Title.Models;
using Orchard.Projections.Models;

namespace Coevery.Projections.Handlers {
    public class ProjectionEntityEventsHandler : IEntityEvents {
        private readonly IProjectionService _projectionService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;

        public ProjectionEntityEventsHandler(
            IProjectionService projectionService,
            IContentDefinitionManager contentDefinitionManager,
            IContentManager contentManager) {
            _projectionService = projectionService;
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
        }

        public void OnCreated(string entityName) {
            var fields = _contentDefinitionManager.GetPartDefinition(entityName).Fields.Select(x => x.Name);
            var viewModel = new ProjectionEditViewModel {
                ItemContentType = entityName,
                DisplayName = entityName + " DefaultView",
                PageRowCount = 50,
                IsDefault = true
            };
            _projectionService.EditPost(0, viewModel, fields);
        }

        public void OnDeleting(string entityName) {
            var projections = _contentManager.Query<ProjectionPart>().List().Where(t => t.As<TitlePart>().Title == entityName);
            foreach (var projection in projections) {
                _contentManager.Remove(projection.ContentItem);
            }
        }
    }
}