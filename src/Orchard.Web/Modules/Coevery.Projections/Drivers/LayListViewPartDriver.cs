using System;
using System.Linq;
using System.Xml.Linq;
using Coevery.Projections.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Projections.Models;

namespace Coevery.Projections.Drivers {

    public class LayListViewPartDriver : ContentPartDriver<ListViewPart>
    {
        protected override DriverResult Editor(ListViewPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            if(updater == null) {
                return null;
            }

            return null;
        }
    }
}