using JetBrains.Annotations;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Handlers;
using Coevery.Data;
using Coevery.Widgets.Models;

namespace Coevery.Widgets.Handlers {
    [UsedImplicitly]
    public class LayerPartHandler : ContentHandler {
        public LayerPartHandler(IRepository<LayerPartRecord> layersRepository) {
            Filters.Add(StorageFilter.For(layersRepository));
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var part = context.ContentItem.As<LayerPart>();

            if (part != null) {
                 context.Metadata.Identity.Add("Layer.LayerName", part.Name);
            }
        }
    }
}