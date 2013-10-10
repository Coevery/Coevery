using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Projections.Models;

namespace Coevery.Projections
{
    public class ListViewShape : IShapeTableProvider {
        public void Discover(ShapeTableBuilder builder) {}

        [Shape]
        public void ngGrid(dynamic Display, TextWriter Output, HtmlHelper Html, IEnumerable<dynamic> Items, String ContentType) {
            Output.WriteLine("<section class=\"gridStyle\" ng-grid=\"gridOptions\"></section>");
        }
    }
}
