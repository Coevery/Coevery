using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.DisplayManagement;
using Coevery.Localization;
using Coevery.Projections.Descriptors.Layout;
using Coevery.Projections.Models;
using Coevery.Projections.Services;

namespace Coevery.Projections.Providers.Layouts.Grid {
    public class DefaultGridLayout : ILayoutProvider {
        private readonly IContentManager _contentManager;
        protected dynamic Shape { get; set; }

        public DefaultGridLayout(IShapeFactory shapeFactory, IContentManager contentManager) {
            _contentManager = contentManager;
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeLayoutContext describe) {
            describe.For("Grids", T("Grids"), T("Grid HTML Layouts"))
                .Element("Default", T("Default"), T("Organizes entity items in a grid."),
                    DisplayLayout,
                    RenderLayout,
                    "DefaultGridLayout"
                );
        }

        public LocalizedString DisplayLayout(LayoutContext context) {
            return T("Default sortable grid style for entity with pager.");
        }

        public dynamic RenderLayout(LayoutContext context, IEnumerable<LayoutComponentResult> layoutComponentResults) {
            
            //int pageRowCount = Convert.ToInt32(context.State["PageRowCount"]);
            //string sortedBy = context.State["SortedBy"];
            //string sortMode = context.State["SortMode"];

            return Shape.DefaultGrid();
        }
    }
}