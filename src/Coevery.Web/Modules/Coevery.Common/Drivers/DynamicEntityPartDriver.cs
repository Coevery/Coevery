using System.Linq;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;

namespace Coevery.Common.Drivers {
    public class DynamicEntityPartDriver : ContentPartDriver<ContentPart> {
        protected override DriverResult Display(ContentPart part, string displayType, dynamic shapeHelper) {
            return GetShapes(part, shapeHelper);
        }

        protected override DriverResult Editor(ContentPart part, dynamic shapeHelper) {
            return GetShapes(part, shapeHelper);
        }

        private DriverResult GetShapes(ContentPart part, dynamic shapeHelper) {
            bool isDynamicRecord = part.ContentItem.Parts.Any(x => x.GetType().Namespace == "Coevery.DynamicTypes.Records");
            if (isDynamicRecord) {
                return Combined(
                    ContentShape("Parts_DynamicEntity_Header", header => header),
                    ContentShape("Parts_DynamicEntity_Actions", actions => actions),
                    ContentShape("Parts_DynamicEntity_Layout",
                        () => shapeHelper.Parts_DynamicEntity_Layout(Layout: GetLayout(part)))
                    );
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