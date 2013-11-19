using Coevery.Common.Models;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;

namespace Coevery.Common.Drivers {
    public class ViewPagePartDriver : ContentPartDriver<ViewPagePart> {
        protected override DriverResult Display(ViewPagePart part, string displayType, dynamic shapeHelper) {
            var contentItem = part.Item;
            return Combined(
                ContentShape("Parts_ViewPage_Header",
                    () => shapeHelper.Parts_ViewPage_Header(Item: contentItem)),
                ContentShape("Parts_ViewPage_Actions",
                    () => shapeHelper.Parts_ViewPage_Actions(Item: contentItem)),
                ContentShape("Parts_ViewPage_Layout",
                    () => shapeHelper.Parts_ViewPage_Layout(Item: contentItem, Layout: GetLayout(contentItem)))
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