using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.OptionSet.Fields;
using Coevery.OptionSet.Helpers;
using Coevery.OptionSet.Models;
using Coevery.OptionSet.Services;
using Coevery.OptionSet.Settings;
using Coevery.OptionSet.ViewModels;
using JetBrains.Annotations;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using Orchard.Localization;
using Orchard.UI.Notify;

namespace Coevery.OptionSet.Drivers {
    [UsedImplicitly]
    public class OptionSetFieldDriver : ContentFieldDriver<OptionSetField> {
        private readonly IOptionSetService _optionSetService;
        public IOrchardServices Services { get; set; }

        public OptionSetFieldDriver(
            IOrchardServices services, 
            IOptionSetService optionSetService,
            IRepository<OptionItemContentItem> repository) {
            _optionSetService = optionSetService;
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(OptionSetField field, ContentPart part) {
            return field.Name;
        }
        protected override DriverResult Display(ContentPart part, OptionSetField field, string displayType, dynamic shapeHelper) {

            return ContentShape("Fields_OptionSetField", GetDifferentiator(field, part),
                () => {
                    var settings = field.PartFieldDefinition.Settings.GetModel<OptionSetFieldSettings>();
                    var optionItems = _optionSetService.GetOptionItemsForContentItem(part.ContentItem.Id, field.Name).ToList();
                    var optionSet = _optionSetService.GetOptionSet(settings.OptionSetId);

                    return shapeHelper.Fields_OptionSetField(
                        ContentField: field,
                        OptionItems: optionItems,
                        Settings: settings,
                        OptionSet: optionSet);
                });
        }

        protected override DriverResult Editor(ContentPart part, OptionSetField field, dynamic shapeHelper) {
            return ContentShape("Fields_OptionSetField_Edit", GetDifferentiator(field, part), () => {
                var settings = field.PartFieldDefinition.Settings.GetModel<OptionSetFieldSettings>();
                var appliedTerms = _optionSetService.GetOptionItemsForContentItem(part.ContentItem.Id, field.Name).Distinct(new TermPartComparer()).ToDictionary(t => t.Id, t => t);
                var optionSet = _optionSetService.GetOptionSet(settings.OptionSetId);
                var optionItems = optionSet != null
                    ? _optionSetService.GetOptionItems(optionSet.Id).Where(t => !string.IsNullOrWhiteSpace(t.Name)).Select(t => t.CreateTermEntry()).ToList()
                    : new List<OptionItemEntry>(0);

                optionItems.ForEach(t => t.IsChecked = appliedTerms.ContainsKey(t.Id));

                var viewModel = new OptionSetFieldViewModel
                {
                    Name = field.Name,
                    DisplayName =  field.DisplayName,
                    OptionItems = optionItems,
                    Settings = settings,
                    SingleTermId = optionItems.Where(t => t.IsChecked).Select(t => t.Id).FirstOrDefault(),
                    OptionSetId = optionSet != null ? optionSet.Id : 0
                };

                return shapeHelper.EditorTemplate(TemplateName: "Fields/OptionSetField", Model: viewModel, Prefix: GetPrefix(field, part));
            });
        }

        protected override DriverResult Editor(ContentPart part, OptionSetField field, IUpdateModel updater, dynamic shapeHelper) {
            var viewModel = new OptionSetFieldViewModel { OptionItems = new List<OptionItemEntry>() };
            
            if(updater.TryUpdateModel(viewModel, GetPrefix(field, part), null, null)) {
                var checkedTerms = viewModel.OptionItems
                    .Where(t => (t.IsChecked || t.Id == viewModel.SingleTermId))
                    .Select(t => GetOrCreateTerm(t, viewModel.OptionSetId, field))
                    .Where(t => t != null).ToList();

                var settings = field.PartFieldDefinition.Settings.GetModel<OptionSetFieldSettings>();
                if (settings.Required && !checkedTerms.Any()) {
                    updater.AddModelError(GetPrefix(field, part), T("The field {0} is mandatory.", T(field.DisplayName)));
                }
                else
                    _optionSetService.UpdateTerms(part.ContentItem, checkedTerms, field.Name);
            }

            return Editor(part, field, shapeHelper);
        }

        protected override void Exporting(ContentPart part, OptionSetField field, ExportContentContext context) {
            var appliedTerms = _optionSetService.GetOptionItemsForContentItem(part.ContentItem.Id, field.Name);

            // stores all content items associated to this field
            var termIdentities = appliedTerms.Select(x => Services.ContentManager.GetItemMetadata(x).Identity.ToString())
                .ToArray();

            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Terms", String.Join(",", termIdentities));
        }

        protected override void Importing(ContentPart part, OptionSetField field, ImportContentContext context) {
            var termIdentities = context.Attribute(field.FieldDefinition.Name + "." + field.Name, "Terms");
            if (termIdentities == null) {
                return;
            }

            var terms = termIdentities
                            .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                            .Select(context.GetItemFromSession)
                            .Where(contentItem => contentItem != null)
                            .ToList();

            _optionSetService.UpdateTerms(part.ContentItem, terms.Select(x => x.As<OptionItemPart>()), field.Name);
        }

        private OptionItemPart GetOrCreateTerm(OptionItemEntry entry, int taxonomyId, OptionSetField field) {
            var term = entry.Id > 0 ? _optionSetService.GetOptionItem(entry.Id) : default(OptionItemPart);

            if (term == null) {
                var settings = field.PartFieldDefinition.Settings.GetModel<OptionSetFieldSettings>();

                if (!Services.Authorizer.Authorize(Permissions.CreateTerm)) {
                    Services.Notifier.Error(T("You're not allowed to create new terms for this taxonomy"));
                    return null;
                }

                var taxonomy = _optionSetService.GetOptionSet(taxonomyId);
                term = _optionSetService.NewTerm(taxonomy);
                term.Container = taxonomy.ContentItem;
                term.Name = entry.Name.Trim();
                term.Selectable = true;

                Services.ContentManager.Create(term, VersionOptions.Published);
                Services.Notifier.Information(T("The {0} term has been created.", term.Name));
            }

            return term;
        }

        protected override void Describe(DescribeMembersContext context) {
            context
                .Member(null, typeof(string), null, T("The option value of the field."));
        }

    }

    internal class TermPartComparer : IEqualityComparer<OptionItemPart> {
        public bool Equals(OptionItemPart x, OptionItemPart y) {
            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(OptionItemPart obj) {
            return obj.Id.GetHashCode();
        }
    }
}
