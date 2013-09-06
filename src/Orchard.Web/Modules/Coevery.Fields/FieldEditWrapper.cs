using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.DisplayManagement.Descriptors;

namespace Coevery.Fields {
    public class FieldEditWrapper : IShapeTableProvider {
        public void Discover(ShapeTableBuilder builder) {
            //builder.Describe("Content_Edit")
            //       .OnDisplaying(displaying => {
            //           var shape = displaying.Shape;
            //           shape.Metadata.Alternates.Add("Content_Edit__Default");
            //       });

            //builder.Describe("Fields_Input_Edit").OnDisplaying(displaying => {
            //    if (!displaying.ShapeMetadata.DisplayType.Contains("Admin")) {
            //        displaying.ShapeMetadata.Wrappers.Add("Content_ControlWrapper");
            //    }
            //});
        }
    }
}