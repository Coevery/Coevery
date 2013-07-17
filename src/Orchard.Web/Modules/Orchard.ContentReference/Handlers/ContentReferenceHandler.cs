using System.Linq;
using Contrib.ContentReference.Fields;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;

namespace Contrib.ContentReference.Handlers {
    [OrchardFeature("Contrib.ContentReference")]
    public class ContentReferenceHandler : ContentHandler {
        private readonly IAuthenticationService _authenticationService;
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public ContentReferenceHandler(
            IAuthenticationService authenticationService,
            IContentManager contentManager,
            IContentDefinitionManager contentDefinitionManager) {

            T = NullLocalizer.Instance;

            _authenticationService = authenticationService;
            _contentManager = contentManager;
            _contentDefinitionManager = contentDefinitionManager;

            //OnActivated<ContentPart>(PropertySetHandlers);
            //OnLoading<ContentPart>((context, part) => LazyLoadHandlers(part));
            //OnVersioning<ContentPart>((context, part, newVersionPart) => LazyLoadHandlers(newVersionPart));
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
            foreach (ContentReferenceField field in part.Fields.Where(f => f.FieldDefinition.Name.Equals("ContentReferenceField"))) {
                var field1 = field;
                field.ContentItemField.Loader(() => field1.ContentId.HasValue ? _contentManager.Get(field1.ContentId.Value) : null);   
            }
        }

        protected static void PropertySetHandlers(ActivatedContentContext context, ContentPart part) {
            // add handlers that will update ID when ContentItem is assigned
            foreach (ContentReferenceField field in part.Fields.Where(f => f.FieldDefinition.Name.Equals("ContentReferenceField"))) {
                var field1 = field;
                field1.ContentItemField.Setter(contentItem => {
                    field1.ContentId = 
                        contentItem == null
                            ? new int?()
                            : contentItem.Id;
                    return contentItem;
                });

                // Force call to setter if we had already set a value
                if (field1.ContentItemField.Value != null)
                    field1.ContentItemField.Value = field1.ContentItemField.Value;
            }
        }
    }
}
