using System.Web.Routing;
using JetBrains.Annotations;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Handlers;
using Coevery.Core.Title.Models;
using Coevery.Data;
using Coevery.Widgets.Models;

namespace Coevery.Widgets.Handlers {
    [UsedImplicitly]
    public class WidgetPartHandler : ContentHandler {
        public WidgetPartHandler(IRepository<WidgetPartRecord> widgetsRepository) {
            Filters.Add(StorageFilter.For(widgetsRepository));

            OnInitializing<WidgetPart>((context, part) => part.RenderTitle = true);
            OnIndexing<TitlePart>((context, part) => context.DocumentIndex.Add("title", part.Title).RemoveTags().Analyze());
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var widget = context.ContentItem.As<WidgetPart>();

            if (widget == null)
                return;

            context.Metadata.DisplayText = widget.Title;

            // create needs to go through the add widget flow (index -> [select layer -> ] add [widget type] to layer)
            context.Metadata.CreateRouteValues = new RouteValueDictionary {
                {"Area", "Coevery.Widgets"},
                {"Controller", "Admin"},
                {"Action", "Index"}
            };
            context.Metadata.EditorRouteValues = new RouteValueDictionary {
                {"Area", "Coevery.Widgets"},
                {"Controller", "Admin"},
                {"Action", "EditWidget"},
                {"Id", context.ContentItem.Id}
            };
            // remove goes through edit widget...
            context.Metadata.RemoveRouteValues = new RouteValueDictionary {
                {"Area", "Coevery.Widgets"},
                {"Controller", "Admin"},
                {"Action", "EditWidget"},
                {"Id", context.ContentItem.Id}
            };
        }
    }
}