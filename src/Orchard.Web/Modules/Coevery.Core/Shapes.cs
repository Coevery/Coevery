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

namespace Coevery.Core {
    public class Shapes : IShapeTableProvider
    {
        private IContentDefinitionManager _contentDefinitionManager;
        public Shapes(IContentDefinitionManager contentDefinitionManager)
        {
            _contentDefinitionManager = contentDefinitionManager;
        }
        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Content_Edit")
                .OnDisplaying(displaying => {
                    var shape = displaying.Shape;
                    shape.Metadata.Alternates.Add("Content_Edit__Default");
                });
        }

        [Shape]
        public void ngGrid(dynamic Display, TextWriter Output, HtmlHelper Html, IEnumerable<dynamic> Items,String ContentType)
        {
            Output.WriteLine("<section class=\"gridStyle\" ng-grid=\"gridOptions\"></section>");
        }
   
    }
}
