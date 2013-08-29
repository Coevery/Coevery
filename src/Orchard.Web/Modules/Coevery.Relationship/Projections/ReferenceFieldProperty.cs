using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Coevery.Relationship.Fields;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Property;
using Orchard.Projections.FieldTypeEditors;
using Orchard.Projections.PropertyEditors;
using Orchard.Projections.Services;
using Orchard.Utility.Extensions;

namespace Coevery.Relationship.Projections {
    public class ReferenceFieldProperty : IPropertyProvider {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IEnumerable<IFieldTypeEditor> _fieldTypeEditors;
        private readonly IPropertyFormater _propertyFormater;
        private readonly IContentManager _contentManager;
        private readonly UrlHelper _urlHelper;

        public ReferenceFieldProperty(
            IContentDefinitionManager contentDefinitionManager,
            IEnumerable<IFieldTypeEditor> fieldTypeEditors,
            IPropertyFormater propertyFormater, 
            IContentManager contentManager, 
            UrlHelper urlHelper) {
            _contentDefinitionManager = contentDefinitionManager;
            _fieldTypeEditors = fieldTypeEditors;
            _propertyFormater = propertyFormater;
            _contentManager = contentManager;
            _urlHelper = urlHelper;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribePropertyContext describe) {
            foreach (var part in _contentDefinitionManager.ListPartDefinitions()) {
                if (!part.Fields.Any()) {
                    continue;
                }

                var descriptor = describe.For(part.Name + "ContentFields", T("{0} Content Fields", part.Name.CamelFriendly()), T("Content Fields for {0}", part.Name.CamelFriendly()));

                foreach (var field in part.Fields) {
                    var localField = field;
                    var localPart = part;

                    if (field.FieldDefinition.Name == typeof (ReferenceField).Name) {
                        var membersContext = new DescribeMembersContext(
                            (storageName, storageType, displayName, description) => {
                                // look for a compatible field type editor
                                IFieldTypeEditor fieldTypeEditor = _fieldTypeEditors.FirstOrDefault(x => x.CanHandle(storageType));

                                descriptor.Element(
                                    type: localPart.Name + "." + localField.Name + "." + storageName ?? "",
                                    name: new LocalizedString(localField.DisplayName + (displayName != null ? ":" + displayName.Text : "")),
                                    description: description ?? T("{0} property for {1}", storageName, localField.DisplayName),
                                    property: (context, contentItem) => Render(context, contentItem, fieldTypeEditor, storageName, storageType, localPart, localField),
                                    display: context => DisplayFilter(context, localPart, localField, storageName),
                                    form: _propertyFormater.GetForm(storageType)
                                    );
                            });

                        membersContext.Member(null, typeof(int), T("Value"), T("The content item id referenced by this field."));
                    }
                }
            }
        }

        public dynamic Render(PropertyContext context, ContentItem contentItem, IFieldTypeEditor fieldTypeEditor, string storageName, Type storageType, ContentPartDefinition part, ContentPartFieldDefinition field) {
            var p = contentItem.Parts.FirstOrDefault( x => x.PartDefinition.Name == part.Name);

            if(p == null) {
                return String.Empty;
            }

            var f = p.Fields.OfType<ReferenceField>().FirstOrDefault(x => x.Name == field.Name);

            if(f == null) {
                return String.Empty;
            }

            var value = f.Storage.Get<int?>(storageName);

            if (value == null) {
                return null;
            }

            var referenceContentItem = _contentManager.Get(value.Value);
            var contentItemMetadata = _contentManager.GetItemMetadata(referenceContentItem);

            var linkTag = new TagBuilder("a");
            linkTag.Attributes.Add("href", _urlHelper.RouteUrl(contentItemMetadata.DisplayRouteValues));
            linkTag.InnerHtml = contentItemMetadata.DisplayText;
            return linkTag.ToString();
        }

        public LocalizedString DisplayFilter(PropertyContext context, ContentPartDefinition part, ContentPartFieldDefinition fieldDefinition, string storageName) {
            return T("Field {0}: {1}", fieldDefinition.Name, String.IsNullOrEmpty(storageName) ? T("Default value").Text : storageName);
        }
    }


}