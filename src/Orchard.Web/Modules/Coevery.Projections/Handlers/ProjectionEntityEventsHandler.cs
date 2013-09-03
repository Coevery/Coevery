using System.Linq;
using Coevery.Core.Services;
using Coevery.Entities.Events;
using Coevery.Projections.Services;
using Coevery.Projections.ViewModels;
using Orchard.ContentManagement.MetaData;

namespace Coevery.Projections.Handlers {
    public class ProjectionEntityEventsHandler : IEntityEvents {
        private readonly IProjectionService _projectionService;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IViewPartService _viewPartService;

        public ProjectionEntityEventsHandler(
            IProjectionService projectionService,
            IContentDefinitionManager contentDefinitionManager,
            IViewPartService viewPartService) {
            _projectionService = projectionService;
            _contentDefinitionManager = contentDefinitionManager;
            _viewPartService = viewPartService;
        }

        public void OnCreated(string entityName) {
            var model = _projectionService.CreateTempProjection(entityName);
            var fields = _contentDefinitionManager.GetPartDefinition(entityName).Fields.Select(x => x.Name);
            var viewModel = new ProjectionEditViewModel {
                Name = entityName,
                DisplayName = entityName + "DefaultView",
                PageRowCount = 50
            };
            _projectionService.EditPost(model.Id, viewModel, fields);
            _viewPartService.SetView(entityName, model.Id);
        }
    }
}