using System.Web.Mvc;
using Coevery.Common.Extensions;
using Coevery.Entities.Services;
using Coevery.Relationship.Fields;
using Coevery.ContentManagement;

namespace Coevery.Relationship.Projections {

    public class ReferenceFieldValueProvider : ContentFieldValueProvider<ReferenceField> {
        private readonly IContentManager _contentManager;
        private readonly IContentDefinitionExtension _contentDefinitionExtension;

        public ReferenceFieldValueProvider(
            IContentManager contentManager,
            IContentDefinitionExtension contentDefinitionExtension) {
            _contentManager = contentManager;
            _contentDefinitionExtension = contentDefinitionExtension;
        }

        public override object GetValue(ContentItem contentItem, ContentField field) {
            var value = field.Storage.Get<int?>(null);

            if (value == null) {
                return null;
            }

            var referenceContentItem = _contentManager.Get(value.Value);
            var contentItemMetadata = _contentManager.GetItemMetadata(referenceContentItem);

            var pluralContentTypeName = _contentDefinitionExtension.GetEntityNames(referenceContentItem.ContentType).CollectionName;

            var linkTag = new TagBuilder("a");
            linkTag.AddCssClass("btn-link");
            linkTag.Attributes.Add("ui-sref", "View({NavigationId: $stateParams.NavigationId, Module: '" + pluralContentTypeName + "', Id: " + value + "})");
            linkTag.InnerHtml = contentItemMetadata.DisplayText;
            return linkTag.ToString();
        }
    }
}