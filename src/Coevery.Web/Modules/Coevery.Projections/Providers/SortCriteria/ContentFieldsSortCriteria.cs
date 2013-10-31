using System.Collections.Generic;
using System.Linq;
using Coevery.Common.Extensions;
using Coevery.Projections.FieldTypeEditors;
using Coevery.ContentManagement.Drivers;
using Coevery.ContentManagement.Handlers;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.Environment.Extensions;
using Coevery.Localization;
using Coevery.Orchard.Projections.Descriptors.SortCriterion;
using Coevery.Orchard.Projections.Providers.SortCriteria;
using Coevery.Orchard.Projections.Services;
using Coevery.Utility.Extensions;

namespace Coevery.Projections.Providers.SortCriteria {
    [CoeverySuppressDependency("Coevery.Projections.Providers.SortCriteria.ContentFieldsSortCriterion")]
    public class ContentFieldsSortCriterion : ISortCriterionProvider {
        private readonly IContentDefinitionExtension _contentDefinitionExtension;
        private readonly IEnumerable<IContentFieldDriver> _contentFieldDrivers;
        private readonly IEnumerable<IConcreteFieldTypeEditor> _fieldTypeEditors;

        public ContentFieldsSortCriterion(
            IContentDefinitionExtension contentDefinitionExtension,
            IEnumerable<IContentFieldDriver> contentFieldDrivers,
            IEnumerable<IConcreteFieldTypeEditor> fieldTypeEditors) {
            _contentDefinitionExtension = contentDefinitionExtension;
            _contentFieldDrivers = contentFieldDrivers;
            _fieldTypeEditors = fieldTypeEditors;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Describe(DescribeSortCriterionContext describe) {
            foreach (var part in _contentDefinitionExtension.ListUserDefinedPartDefinitions()) {
                if(!part.Fields.Any()) {
                    continue;
                }

                var descriptor = describe.For(part.Name + "ContentFields", T("{0} Content Fields", part.Name.CamelFriendly()), T("Content Fields for {0}", part.Name.CamelFriendly()));

                foreach(var field in part.Fields) {
                    var localField = field;
                    var localPart = part;
                    var drivers = _contentFieldDrivers.Where(x => x.GetFieldInfo().Any(fi => fi.FieldTypeName == localField.FieldDefinition.Name)).ToList();

                    var membersContext = new DescribeMembersContext(
                        (storageName, storageType, displayName, description) => {
                            // look for a compatible field type editor
                            IConcreteFieldTypeEditor fieldTypeEditor = _fieldTypeEditors.FirstOrDefault(x => x.CanHandle(localField.FieldDefinition.Name, storageType))
                                                                       ?? _fieldTypeEditors.FirstOrDefault(x => x.CanHandle(storageType));

                            if (fieldTypeEditor == null) return;

                            descriptor.Element(
                                type: localPart.Name + "." + localField.Name + "." + storageName ?? "",
                                name: new LocalizedString(localField.DisplayName + (displayName != null ? ":" + displayName.Text : "")),
                                description: description ?? T("{0} property for {1}", storageName, localField.DisplayName),
                                sort: context => fieldTypeEditor.ApplySortCriterion(context, storageName, storageType, localPart, localField),
                                display: context => DisplaySortCriterion(context, localPart, localField),
                                form: SortCriterionFormProvider.FormName);
                        });
                    
                    foreach(var driver in drivers) {
                        driver.Describe(membersContext);
                    }
                }
            }
        }

        public LocalizedString DisplaySortCriterion(SortCriterionContext context, ContentPartDefinition part, ContentPartFieldDefinition fieldDefinition) {
            bool ascending = (bool)context.State.Sort;

            return ascending
                       ? T("Ordered by field {0}, ascending", fieldDefinition.Name)
                       : T("Ordered by field {0}, descending", fieldDefinition.Name);

        }

    }
}