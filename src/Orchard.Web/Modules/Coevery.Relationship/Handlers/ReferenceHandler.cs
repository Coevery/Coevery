using System.Linq;
using Coevery.Relationship.Fields;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;

namespace Coevery.Relationship.Handlers {
    public class ReferenceHandler : ContentHandler {
        private readonly IContentManager _contentManager;
        public ReferenceHandler(
            IContentManager contentManager) {
            T = NullLocalizer.Instance;
            _contentManager = contentManager;
        }

        public Localizer T { get; set; }

        protected override void Activated(ActivatedContentContext context) {
            foreach(var part in context.ContentItem.Parts) {
                PropertySetHandlers(context, part);
            }
        }

        protected override void Loading(LoadContentContext context) {
            foreach (var part in context.ContentItem.Parts) {
                LazyLoadHandlers(part);
            }
        }

        protected override void Versioning(VersionContentContext context) {
            foreach (var part in context.BuildingContentItem.Parts) {
                LazyLoadHandlers(part);
            }
        }

        protected void LazyLoadHandlers(ContentPart part) {
            // add handlers that will load content for id's just-in-time
            foreach (ReferenceField field in part.Fields.Where(f => f.FieldDefinition.Name.Equals("ReferenceField"))) {
                var field1 = field;
                field.ContentItemField.Loader(() => field1.Value.HasValue ? _contentManager.Get(field1.Value.Value) : null);   
            }
        }

        protected static void PropertySetHandlers(ActivatedContentContext context, ContentPart part) {
            // add handlers that will update ID when ContentItem is assigned
            foreach (ReferenceField field in part.Fields.Where(f => f.FieldDefinition.Name.Equals("ReferenceField"))) {
                var field1 = field;
                field1.ContentItemField.Setter(contentItem => {
                    field1.Value = contentItem == null ? new int?() : contentItem.Id;
                    return contentItem;
                });

                // Force call to setter if we had already set a value
                if (field1.ContentItemField.Value != null)
                    field1.ContentItemField.Value = field1.ContentItemField.Value;
            }
        }
    }
}
