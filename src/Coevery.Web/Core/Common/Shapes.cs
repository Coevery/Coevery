using System;
using System.Web;
using System.Web.Mvc;
using Coevery.DisplayManagement;
using Coevery.DisplayManagement.Descriptors;
using Coevery.Localization;
using Coevery.Mvc.Html;

namespace Coevery.Core.Common {
    public class Shapes : IShapeTableProvider {
        public Shapes() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Discover(ShapeTableBuilder builder) {
            builder.Describe("Body_Editor")
                .OnDisplaying(displaying => {
                    string flavor = displaying.Shape.EditorFlavor;
                    displaying.ShapeMetadata.Alternates.Add("Body_Editor__" + flavor);
                });
        }

        [Shape]
        public IHtmlString PublishedState(dynamic Display, DateTime createdDateTimeUtc, DateTime? publisheddateTimeUtc) {
            if (!publisheddateTimeUtc.HasValue) {
                return T("Draft");
            }

            return Display.DateTime(DateTimeUtc: createdDateTimeUtc);
        }

        [Shape]
        public IHtmlString PublishedWhen(dynamic Display, DateTime? dateTimeUtc) {
            if (dateTimeUtc == null)
                return T("as a Draft");

            return Display.DateTimeRelative(dateTimeUtc: dateTimeUtc);
        }
    }
}
