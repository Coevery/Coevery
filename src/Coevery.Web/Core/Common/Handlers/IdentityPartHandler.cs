using System;
using JetBrains.Annotations;
using Coevery.ContentManagement;
using Coevery.Core.Common.Models;
using Coevery.Data;
using Coevery.ContentManagement.Handlers;

namespace Coevery.Core.Common.Handlers {
    [UsedImplicitly]
    public class IdentityPartHandler : ContentHandler {
        public IdentityPartHandler(IRepository<IdentityPartRecord> identityRepository,
            IContentManager contentManager) {
            Filters.Add(StorageFilter.For(identityRepository));
            OnInitializing<IdentityPart>(AssignIdentity);
        }

        protected void AssignIdentity(InitializingContentContext context, IdentityPart part) {
            part.Identifier = Guid.NewGuid().ToString("n");
        }

        protected override void GetItemMetadata(GetContentItemMetadataContext context) {
            var part = context.ContentItem.As<IdentityPart>();

            if (part != null) {
                context.Metadata.Identity.Add("Identifier", part.Identifier);
            }
        }
    }
}