using System;
using Coevery.Fields.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Coevery.Fields.Fields;
using Coevery.Fields.Settings;
using Orchard.Localization;

namespace Coevery.Fields.Drivers {
    public class OptionSetFieldDriver : ContentFieldDriver<OptionSetField> {
        public IOrchardServices Services { get; set; }
        private const string TemplateName = "Fields/OptionSet.Edit";
        private readonly IOptionItemService _optionItemService;
        private readonly IFieldDependencyService _fieldDependencyService;

        public OptionSetFieldDriver(IOrchardServices services,
            IOptionItemService optionItemService,
            IFieldDependencyService fieldDependencyService) {
            _optionItemService = optionItemService;
            _fieldDependencyService = fieldDependencyService;
            Services = services;
            T = NullLocalizer.Instance;
            DisplayName = "OptionSet";
            Description = "Allows users to select a value or values from a list you define.";
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(OptionSetField field, ContentPart part) {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, OptionSetField field, string displayType, dynamic shapeHelper) {
            return ContentShape("Fields_OptionSet", GetDifferentiator(field, part), () => {
                var settings = field.PartFieldDefinition.Settings.GetModel<OptionSetFieldSettings>();
                return shapeHelper.Fields_OptionSet().Settings(settings);
            });
        }

        //Create or edit
        protected override DriverResult Editor(ContentPart part, OptionSetField field, dynamic shapeHelper) {
            //if the content item is new, assign the default value
            var settings = field.PartFieldDefinition.Settings.GetModel<OptionSetFieldSettings>();

            if (field.Items == null) {
                field.Items = _optionItemService.GetItemsForField(settings.OptionSetId);
            }

            if (settings.DependencyMode == DependentType.Dependent && field.DisplayItems == null) {
                field.DisplayItems = _fieldDependencyService.GetDependencyMap(settings.OptionSetId);
            }

            //field.OptionValue = _optionItemService.GetSelectedSet(field.Value);

            return ContentShape("Fields_OptionSet_Edit", GetDifferentiator(field, part),
                 () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: field, Prefix: GetPrefix(field, part)));
        }

        //Creat or edit Post
        protected override DriverResult Editor(ContentPart part, OptionSetField field, IUpdateModel updater, dynamic shapeHelper) {
            if (updater.TryUpdateModel(field, GetPrefix(field, part), null, null)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<OptionSetFieldSettings>();
                if (settings.Required && field.OptionValue == null) {
                    updater.AddModelError(field.Name, T("The field {0} is required.", T(field.DisplayName)));
                }
                if (field.OptionValue == null) {
                    return Editor(part, field, shapeHelper);
                }
                //field.Value = _optionItemService.AlterSet(field.Value ?? 0, field.OptionValue);
                if (field.Value == null) {
                    updater.AddModelError(field.Name, T("Option set creation failed."));
                }
            }
            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, OptionSetField field, ImportContentContext context) {
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Value", v => field.Value = v);
        }

        protected override void Exporting(ContentPart part, OptionSetField field, ExportContentContext context) {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Value", field.Value);
        }

        protected override void Describe(DescribeMembersContext context) {
            context.Member(null, typeof(string), T("Value"), T("The string value of the field."));
        }
    }
}
