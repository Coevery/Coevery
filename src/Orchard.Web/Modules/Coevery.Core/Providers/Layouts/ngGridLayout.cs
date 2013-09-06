using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Layout;
using Orchard.Projections.Models;
using Orchard.Projections.Services;

namespace Orchard.Projections.Providers.Layouts {
    public class ngGridLayout : ILayoutProvider {
        private readonly IContentManager _contentManager;
        protected dynamic Shape { get; set; }

        public ngGridLayout(IShapeFactory shapeFactory, IContentManager contentManager) {
            _contentManager = contentManager;
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeLayoutContext describe) {
            describe.For("Html", T("Html"),T("Html Layouts"))
                .Element("ngGrid", T("ngGrid"), T("Organizes content items in a ng-grid."),
                    DisplayLayout,
                    RenderLayout,
                    "ngGridLayout"
                );
        }

        public LocalizedString DisplayLayout(LayoutContext context) {
            string columns = context.State.Columns;
            bool horizontal = Convert.ToString(context.State.Alignment) != "vertical";

            return horizontal
                       ? T("{0} columns grid", columns)
                       : T("{0} lines grid", columns);
        }

        public dynamic RenderLayout(LayoutContext context, IEnumerable<LayoutComponentResult> layoutComponentResults)
        {
            int columns = Convert.ToInt32(context.State.Columns.Value);
            bool horizontal = Convert.ToString(context.State.Alignment) != "vertical";
            string rowClass = context.State.RowClass;
            string gridClass = context.State.GridClass;
            string gridId = context.State.GridId;

            string contentType = string.Empty;
            if (layoutComponentResults.Any())
                contentType = layoutComponentResults.First().ContentItem.TypeDefinition.Name;

            IEnumerable<dynamic> shapes =
                context.LayoutRecord.Display == (int)LayoutRecord.Displays.Content
                    ? layoutComponentResults.Select(x => _contentManager.BuildDisplay(x.ContentItem, context.LayoutRecord.DisplayType))
                    : layoutComponentResults.Select(x => x.Properties);

            return Shape.ngGrid(Items: shapes, ContentType: contentType);
        }
    }
}