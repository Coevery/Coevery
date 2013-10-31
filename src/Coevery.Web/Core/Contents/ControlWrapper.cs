using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.DisplayManagement.Descriptors;
using Coevery.Environment.Extensions;

namespace Coevery.Core.Contents {
    [CoeveryFeature("Contents.ControlWrapper")]
    public class ControlWrapper : IShapeTableProvider {
        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Content").OnDisplaying(displaying => {
                if (!displaying.ShapeMetadata.DisplayType.Contains("Admin")) {
                    displaying.ShapeMetadata.Wrappers.Add("Content_ControlWrapper");
                }
            });
        }
    }
}