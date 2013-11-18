using System.Linq;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;

namespace Coevery.Common.Drivers {
    public class LayoutStringDriver : ContentPartDriver<ContentPart> {
        protected override DriverResult Display(ContentPart part, string displayType, dynamic shapeHelper) {
            bool isDynamicRecord = part.ContentItem.Parts.Any(x => x.GetType().Namespace == "Coevery.DynamicTypes.Records");
            if (isDynamicRecord) {
                string layout = GetLayout(part);
                return ContentShape("LayoutString",
                    () => shapeHelper.LayoutString(Layout: layout));
            }

            return null;
        }

        protected override DriverResult Editor(ContentPart part, dynamic shapeHelper) {
            bool isDynamicRecord = part.ContentItem.Parts.Any(x => x.GetType().Namespace == "Coevery.DynamicTypes.Records");
            if (isDynamicRecord) {
                string layout = GetLayout(part);
                return ContentShape("LayoutString_Edit",
                    () => shapeHelper.LayoutString(Layout: layout));
            }

            return null;
        }

        private static string GetLayout(ContentPart part) {
            var contentTypeDefinition = part.TypeDefinition;
            return contentTypeDefinition.Settings.ContainsKey("Layout")
                ? contentTypeDefinition.Settings["Layout"]
                : null;
        }
    }
}