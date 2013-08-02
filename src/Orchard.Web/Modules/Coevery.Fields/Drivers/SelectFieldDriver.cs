using System;
using System.Linq;
using Coevery.Fields.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Coevery.Fields.Fields;
using Coevery.Fields.Settings;
using Orchard.Localization;
using Orchard.Data;
using Coevery.Fields.Records;

namespace Coevery.Fields.Drivers {
    public class SelectFieldDriver : ContentFieldDriver<SelectField> {
        public IOrchardServices Services { get; set; }
        private const string TemplateName = "Fields/Select.Edit";
        private readonly IOptionItemService _optionItemService;

        public SelectFieldDriver(IOrchardServices services,
            IOptionItemService optionItemService) {
            _optionItemService = optionItemService;
            Services = services;
            T = NullLocalizer.Instance;
            DisplayName = "Select";
            Description = "Allows users to select a value or values from a list you define.";
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(SelectField field, ContentPart part) {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, SelectField field, string displayType, dynamic shapeHelper) {
            return ContentShape("Fields_Select", GetDifferentiator(field, part), () => {
                var settings = field.PartFieldDefinition.Settings.GetModel<SelectFieldSettings>();
                return shapeHelper.Fields_Select().Settings(settings);
            });
        }

        //Create or edit
        protected override DriverResult Editor(ContentPart part, SelectField field, dynamic shapeHelper) {
            //if the content item is new, assign the default value
            var settings = field.PartFieldDefinition.Settings.GetModel<SelectFieldSettings>();
            if (string.IsNullOrWhiteSpace(field.Value)) { 
            }
            if (field.Items == null) {
                field.Items = _optionItemService.GetItemsForField(settings.FieldSettingId);
            }

            return ContentShape("Fields_Select_Edit", GetDifferentiator(field, part),
                 () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: field, Prefix: GetPrefix(field, part)));
        }

        //Creat or edit Post
        protected override DriverResult Editor(ContentPart part, SelectField field, IUpdateModel updater, dynamic shapeHelper) {
            if (updater.TryUpdateModel(field, GetPrefix(field, part), null, null)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<SelectFieldSettings>();
                if (settings.Required && !string.IsNullOrWhiteSpace(field.Value)) {
                    updater.AddModelError(field.Name, T("The field {0} is required.", T(field.DisplayName)));
                }
            }
            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, SelectField field, ImportContentContext context) {
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Value", v => field.Value = (v));
        }

        protected override void Exporting(ContentPart part, SelectField field, ExportContentContext context) {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Value", field.Value);
        }

        protected override void Describe(DescribeMembersContext context) {
            context.Member(null, typeof(string), T("Value"), T("The string value of the field."));
        }
    }
}
