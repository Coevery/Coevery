using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using Coevery.ContentManagement;
using Coevery.Projections.Descriptors.Layout;
using Coevery.Projections.FieldTypeEditors;
using Coevery.Projections.Models;
using Coevery.ContentManagement.Drivers;
using Coevery.Forms.Services;
using Coevery.Projections.Services;

namespace Coevery.Projections.Drivers {

    public class ListViewPartDriver : ContentPartDriver<ListViewPart> {
        private readonly IFormManager _formManager;
        private readonly IEnumerable<IFieldTypeEditor> _fieldTypeEditors;
        private readonly IProjectionManager _projectionManager;

        public ListViewPartDriver(IFormManager formManager,
            IProjectionManager projectionManager,
            ICoeveryServices services,
            IEnumerable<IFieldTypeEditor> fieldTypeEditors) {
            _formManager = formManager;
            _projectionManager = projectionManager;
            _fieldTypeEditors = fieldTypeEditors;
        }

        protected override DriverResult Display(ListViewPart part, string displayType, dynamic shapeHelper) {
            var editors = _fieldTypeEditors
                .Select(x => x.FormName)
                .Distinct()
                .Select(x => _formManager.Build(x));
            var layout = part.ContentItem.As<ProjectionPart>().Record.LayoutRecord;
            var layoutDescriptor = _projectionManager.DescribeLayouts()
                .SelectMany(descr => descr.Descriptors)
                .FirstOrDefault(descr => descr.Category == layout.Category && descr.Type == layout.Type);
            if (layoutDescriptor == null) {
                throw new InstanceNotFoundException("Layout not found!");
            }

            var renderLayoutContext = new LayoutContext {
                        State = FormParametersHelper.FromString(layout.State),
                        LayoutRecord = layout,
                    };
            var layoutResult = layoutDescriptor.Render(renderLayoutContext, null);
            return Combined(
                ContentShape("Parts_ListView",
                () => shapeHelper.Parts_ListView(Grid: layoutResult, FilterEditors: editors)),
                ContentShape("Parts_ListView_Buttons", buttons => buttons),
                ContentShape("Parts_ListView_Filters", filters => filters),
                ContentShape("Parts_ListView_FilterContent", filterContent => filterContent),
                ContentShape("Parts_ListView_Views", views => views),
                ContentShape("Parts_ListView_Search", search => search)
                );
        }
    }
}