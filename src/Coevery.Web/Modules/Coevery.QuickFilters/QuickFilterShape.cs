using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData;
using Coevery.DisplayManagement.Descriptors;
using Coevery.OptionSet.Services;
using Coevery.Projections.Descriptors.Filter;
using Coevery.Projections.Services;

namespace Coevery.QuickFilters {
    public class QuickFilterShape : IShapeTableProvider {
        private readonly IProjectionManager _projectionManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;
        private readonly IOptionSetService _optionSetService;

        public QuickFilterShape(
            IProjectionManager projectionManager,
            IContentDefinitionManager contentDefinitionManager,
            IContentManager contentManager,
            IOptionSetService optionSetService) {
            _projectionManager = projectionManager;
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;
            _optionSetService = optionSetService;
        }

        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("QuickFilter_ReferenceFilter")
                .OnDisplaying(displaying => {
                    var shape = displaying.Shape;
                    FilterDescriptor filter = shape.Filter;
                    string filterType = filter.Type;
                    var args = filterType.Split('.');
                    string partName = args[0];
                    string fieldName = args[1];

                    var partDefinition = _contentDefinitionManager.GetPartDefinition(partName);
                    if (partDefinition == null) {
                        return;
                    }
                    var fieldDefinition = partDefinition.Fields.FirstOrDefault(x => x.Name == fieldName);
                    if (fieldDefinition == null) {
                        return;
                    }
                    var queryId = int.Parse(fieldDefinition.Settings["ReferenceFieldSettings.QueryId"]);

                    shape.ListItems = _projectionManager
                        .GetContentItems(queryId)
                        .Select(x => new SelectListItem {
                            Text = _contentManager.GetItemMetadata(x).DisplayText,
                            Value = x.Id.ToString(CultureInfo.InvariantCulture)
                        });
                });

            builder.Describe("QuickFilter_OptionSetFilter")
                .OnDisplaying(displaying => {
                    var shape = displaying.Shape;
                    FilterDescriptor filter = shape.Filter;
                    string filterType = filter.Type;
                    var args = filterType.Split('.');
                    string partName = args[0];
                    string fieldName = args[1];

                    var partDefinition = _contentDefinitionManager.GetPartDefinition(partName);
                    if (partDefinition == null) {
                        return;
                    }
                    var fieldDefinition = partDefinition.Fields.FirstOrDefault(x => x.Name == fieldName);
                    if (fieldDefinition == null) {
                        return;
                    }
                    var optionSetId = int.Parse(fieldDefinition.Settings["OptionSetFieldSettings.OptionSetId"]);
                    shape.ListItems = _optionSetService.GetOptionItems(optionSetId)
                        .Select(x => new SelectListItem {
                            Text = x.Name,
                            Value = x.Id.ToString(CultureInfo.InvariantCulture)
                        });
                });
        }
    }
}