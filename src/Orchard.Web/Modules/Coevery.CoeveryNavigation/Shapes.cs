using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using System.IO;
using System.Web.Mvc;

namespace Coevery.CoeveryNavigation {
    public class Shapes : IShapeTableProvider
    {
        private IContentDefinitionManager _contentDefinitionManager;
        public Shapes(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }
        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Parts_MenuItem_Edit")
                .OnDisplaying(displaying => {
                    var shape = displaying.Shape;
                    shape.Metadata.Alternates.Add("Parts_MenuItem_Edit__Default");
                });
            builder.Describe("Parts_MenuWidget_Edit")
               .OnDisplaying(displaying =>
               {
                   var shape = displaying.Shape;
                   shape.Metadata.Alternates.Add("Parts_MenuWidget_Edit__Default");
               });
            builder.Describe("Parts_Navigation_AdminMenu_Edit")
               .OnDisplaying(displaying =>
               {
                   var shape = displaying.Shape;
                   shape.Metadata.Alternates.Add("Parts_Navigation_AdminMenu_Edit__Default");
               });
            builder.Describe("Parts_Navigation_Menu_Edit")
               .OnDisplaying(displaying =>
               {
                   var shape = displaying.Shape;
                   shape.Metadata.Alternates.Add("Parts_Navigation_Menu_Edit__Default");
               });
            builder.Describe("Parts_ShapeMenuItemPart_Edit")
              .OnDisplaying(displaying =>
              {
                  var shape = displaying.Shape;
                  shape.Metadata.Alternates.Add("Parts_ShapeMenuItemPart_Edit__Default");
              });
        }

        
   
    }
}
