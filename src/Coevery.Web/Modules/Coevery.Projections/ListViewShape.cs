using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData;
using Coevery.DisplayManagement;
using Coevery.DisplayManagement.Descriptors;
using Coevery.Projections.Models;

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
