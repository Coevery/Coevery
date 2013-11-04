using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.DisplayManagement.Descriptors;
using Coevery.Environment.Extensions;

namespace Coevery.Widgets {
    [CoeveryFeature("Coevery.Widgets.ControlWrapper")]
    public class ControlWrapper : IShapeTableProvider {
        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Widget")
                .Configure(descriptor => {
                    descriptor.Wrappers.Add("Widget_ControlWrapper");
                });
        }
    }
}