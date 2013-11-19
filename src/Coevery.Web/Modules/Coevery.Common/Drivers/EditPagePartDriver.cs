using Coevery.Common.Models;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;

namespace Coevery.Common.Drivers {
    public class EditPagePartDriver : ContentPartDriver<EditPagePart> {
        protected override DriverResult Editor(EditPagePart part, dynamic shapeHelper) {
            var contentItem = part.Item;
            return Combined(
                ContentShape("Parts_EditPage_Header",
                    () => shapeHelper.Parts_EditPage_Header(Item: contentItem)),
                ContentShape("Parts_EditPage_Actions",
                    () => shapeHelper.Parts_EditPage_Actions(Item: contentItem)),
                ContentShape("Parts_EditPage_Layout",
                    () => shapeHelper.Parts_EditPage_Layout(Item: contentItem, Layout: GetLayout(contentItem)))
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