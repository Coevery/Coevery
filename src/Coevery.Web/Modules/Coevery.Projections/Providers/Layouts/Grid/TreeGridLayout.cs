using System;
using System.Collections.Generic;
using Coevery.ContentManagement;
using Coevery.DisplayManagement;
using Coevery.Localization;
using Coevery.Projections.Descriptors.Layout;
using Coevery.Projections.Services;

namespace Coevery.Projections.Providers.Layouts.Grid {
    public class TreeGridLayout : ILayoutProvider {
        private readonly IContentManager _contentManager;
        protected dynamic Shape { get; set; }

        public TreeGridLayout(IShapeFactory shapeFactory, IContentManager contentManager) {
            _contentManager = contentManager;
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeLayoutContext describe) {
            describe.For("Grids", T("Grids"), T("Grid HTML Layouts"))
                .Element("Tree", T("Tree"), T("Organizes entity items in a tree grid."),
                    DisplayLayout,
                    RenderLayout,
                    "TreeGridLayout"
                );
        }

        public LocalizedString DisplayLayout(LayoutContext context) {
            return T("Tree grid style for entity.");
        }

        public dynamic RenderLayout(LayoutContext context, IEnumerable<LayoutComponentResult> layoutComponentResults) {
            string expandField = context.State["ExpandField"];
            string parentField = context.State["ParentField"];
            return Shape.TreeGrid(State:
                new {
                    ExpandColumn = expandField.GetFieldName(),
                    ExpandColClick = false,
                    treeGridModel = "adjacency",
                    treeGrid = true,
                    loadonce = true,
                    cmTemplate = new {
                        sortable = false
                    }
                });
        }
    }
}