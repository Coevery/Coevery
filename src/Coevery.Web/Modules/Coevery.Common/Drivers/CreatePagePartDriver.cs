using Coevery.Common.Models;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;

namespace Coevery.Common.Drivers {
    public class CreatePagePartDriver : ContentPartDriver<CreatePagePart> {
        protected override DriverResult Editor(CreatePagePart part, dynamic shapeHelper) {
            var contentItem = part.Item;
            return Combined(
                ContentShape("Parts_CreatePage_Header",
                    () => shapeHelper.Parts_CreatePage_Header(Item: contentItem)),
                ContentShape("Parts_CreatePage_Actions",
                    () => shapeHelper.Parts_CreatePage_Actions(Item: contentItem)),
                ContentShape("Parts_CreatePage_Layout",
                    () => shapeHelper.Parts_CreatePage_Layout(Item: contentItem, Layout: GetLayout(contentItem)))
                );
        }

        private static string GetLayout(ContentItem item) {
            var contentTypeDefinition = item.TypeDefinition;
            return contentTypeDefinition.Settings.ContainsKey("Layout")
                ? contentTypeDefinition.Settings["Layout"]
                : null;
        }
    }
}