using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.Common.Extensions;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;
using Coevery.ContentManagement.Handlers;
using Coevery.ContentManagement.MetaData;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.Entities.Services;
using Coevery.Localization;
using Coevery.Logging;
using Coevery.Projections.Descriptors.Property;
using Coevery.Projections.FieldTypeEditors;
using Coevery.Projections.PropertyEditors;
using Coevery.Projections.Services;
using Coevery.Relationship.Settings;
using Coevery.Utility.Extensions;

namespace Coevery.Relationship.Projections {
    public class ReferenceFieldProperties : IPropertyProvider {
        private readonly IContentDefinitionExtension _contentDefinitionExtension;
        private readonly IEnumerable<IContentFieldDriver> _contentFieldDrivers;
        private readonly IEnumerable<IFieldTypeEditor> _fieldTypeEditors;
        private readonly IEnumerable<IContentFieldValueProvider> _contentFieldValueProviders;
        private readonly IPropertyFormater _propertyFormater;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentManager _contentManager;

        public ReferenceFieldProperties(
            IContentDefinitionExtension contentDefinitionExtension,
            IEnumerable<IContentFieldDriver> contentFieldDrivers,
            IEnumerable<IFieldTypeEditor> fieldTypeEditors,
            IPropertyFormater propertyFormater,
            IEnumerable<IContentFieldValueProvider> contentFieldValueProviders,
            IContentDefinitionManager contentDefinitionManager, 
            IContentManager contentManager) {
            _contentDefinitionExtension = contentDefinitionExtension;
            _contentFieldDrivers = contentFieldDrivers;
            _fieldTypeEditors = fieldTypeEditors;
            _propertyFormater = propertyFormater;
            _contentFieldValueProviders = contentFieldValueProviders;
            _contentDefinitionManager = contentDefinitionManager;
            _contentManager = contentManager;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public void Describe(DescribePropertyContext describe) {
            foreach (var part in _contentDefinitionExtension.ListUserDefinedPartDefinitions()) {
                if (!part.Fields.Any()) {
                    continue;
                }

                var descriptor = describe.For(part.Name + "ContentFields", T("{0} Content Fields", part.Name.CamelFriendly()), T("Content Fields for {0}", part.Name.CamelFriendly()));

                foreach (var field in part.Fields) {
                    if (field.FieldDefinition.Name != "ReferenceField") {
                        continue;
                    }
                    var settings = field.Settings.GetModel<ReferenceFieldSettings>();
                    var referencePart = _contentDefinitionManager.GetPartDefinition(settings.ContentTypeName.ToPartName());

                    foreach (var referenceField in referencePart.Fields) {
                        var parentField = field;
                        var parentPart = part;
                        var localField = referenceField;
                        var drivers = _contentFieldDrivers.Where(x => x.GetFieldInfo().Any(fi => fi.FieldTypeName == localField.FieldDefinition.Name)).ToList();

                        var membersContext = new DescribeMembersContext(
                            (storageName, storageType, displayName, description) => {
                                // look for a compatible field type editor
                                IFieldTypeEditor fieldTypeEditor = _fieldTypeEditors.FirstOrDefault(x => x.CanHandle(storageType));

                                descriptor.Element(
                                    type: parentPart.Name + "." + parentField.Name + "." + localField.Name + "." + storageName ?? "",
                                    name: new LocalizedString(parentField.DisplayName + "." + localField.DisplayName + (displayName != null ? ":" + displayName.Text : "")),
                                    description: description ?? T("{0} property for {1}", storageName, localField.DisplayName),
                                    property: (context, contentItem) => {
                                        var parentFieldValue = GetReferenceFieldValue(contentItem, storageName, parentPart, parentField);
                                        if (parentFieldValue.HasValue) {
                                            var referenceContentItem = _contentManager.Get(parentFieldValue.Value);
                                            return Render(context, referenceContentItem, fieldTypeEditor, storageName, storageType, referencePart, localField);
                                        }
                                        return string.Empty;
                                    },
                                    display: context => DisplayFilter(context, parentPart, localField, storageName),
                                    form: _propertyFormater.GetForm(storageType)
                                    );
                            });

                        foreach (var driver in drivers) {
                            driver.Describe(membersContext);
                        }
                    }
                }
            }
        }

        public dynamic Render(PropertyContext context, ContentItem contentItem, IFieldTypeEditor fieldTypeEditor, string storageName, Type storageType, ContentPartDefinition part, ContentPartFieldDefinition field) {
            var p = contentItem.Parts.FirstOrDefault(x => x.PartDefinition.Name == part.Name);

            if (p == null) {
                return String.Empty;
            }

            var f = p.Fields.FirstOrDefault(x => x.Name == field.Name);

            if (f == null) {
                return String.Empty;
            }

            object value = null;

            _contentFieldValueProviders.Invoke(provider => {
                var result = provider.GetValue(contentItem, f);
                if (result != null) {
                    value = result;
                }
            }, Logger);

            if (value == null) {
                value = f.Storage.Get<object>(storageName);
            }

            if (value == null) {
                return null;
            }

            // call specific formatter rendering
            return _propertyFormater.Format(f, storageType, value, context.State);
        }

        public int? GetReferenceFieldValue(ContentItem contentItem, string storageName, ContentPartDefinition part, ContentPartFieldDefinition field) {
            var p = contentItem.Parts.FirstOrDefault(x => x.PartDefinition.Name == part.Name);

            if (p == null) {
                return null;
            }

            var f = p.Fields.FirstOrDefault(x => x.Name == field.Name);

            if (f == null) {
                return null;
            }
            return f.Storage.Get<int?>(storageName);
        }

        public LocalizedString DisplayFilter(PropertyContext context, ContentPartDefinition part, ContentPartFieldDefinition fieldDefinition, string storageName) {
            return T("Field {0}: {1}", fieldDefinition.Name, String.IsNullOrEmpty(storageName) ? T("Default value").Text : storageName);
        }
    }
}