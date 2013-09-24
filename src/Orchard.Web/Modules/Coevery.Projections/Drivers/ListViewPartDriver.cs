using System.Collections.Generic;
using System.Linq;
using Coevery.Projections.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.Forms.Services;
using Orchard.Projections.FieldTypeEditors;

namespace Coevery.Projections.Drivers {

    public class ListViewPartDriver : ContentPartDriver<ListViewPart> {
        private readonly IFormManager _formManager;
        private readonly IEnumerable<IFieldTypeEditor> _fieldTypeEditors;

        public ListViewPartDriver(IFormManager formManager,
            IEnumerable<IFieldTypeEditor> fieldTypeEditors) {
            _formManager = formManager;
            _fieldTypeEditors = fieldTypeEditors;
        }

        protected override DriverResult Display(ListViewPart part, string displayType, dynamic shapeHelper) {
            var editors = _fieldTypeEditors
                .Select(x => x.FormName)
                .Distinct()
                .Select(x => _formManager.Build(x));
            return Combined(
                ContentShape("Parts_ListView",
                    () => shapeHelper.Parts_ListView(FilterEditors: editors)),
                ContentShape("Parts_ListView_Buttons", buttons => buttons),
                ContentShape("Parts_ListView_Filters", filters => filters),
                ContentShape("Parts_ListView_FilterContent", filterContent => filterContent),
                ContentShape("Parts_ListView_Views", views => views),
                ContentShape("Parts_ListView_Search", search => search)
                );
        }
    }
}