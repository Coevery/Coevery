using System;
using System.Linq;
using System.Xml.Linq;
using Coevery.Projections.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Projections.Models;

namespace Coevery.Projections.Drivers {

    public class LayoutPropertyPartDriver : ContentPartDriver<LayoutPropertyPart>
    {
        protected override DriverResult Editor(LayoutPropertyPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            if(updater == null) {
                return null;
            }

            return null;
        }
    }
}