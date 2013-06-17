using System;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Coevery.Fields.Fields;
using Coevery.Fields.Settings;
using Orchard.Localization;

namespace Coevery.Fields.Drivers {
    public class SelectFieldDriver : ContentFieldDriver<SelectField> {
        public IOrchardServices Services { get; set; }
        private const string TemplateName = "Fields/Select.Edit";

        public SelectFieldDriver(IOrchardServices services) {
            Services = services;
            T = NullLocalizer.Instance;
            DisplayName = "Select";
            Description = "Allows users to select a value from a list you define.";
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
                return shapeHelper.Fields_Boolean().Settings(settings);
            });
        }

        protected override DriverResult Editor(ContentPart part, SelectField field, dynamic shapeHelper) {
           return ContentShape("Fields_Select_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: field, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, SelectField field, IUpdateModel updater, dynamic shapeHelper) {
            if (updater.TryUpdateModel(field, GetPrefix(field, part), null, null)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<SelectFieldSettings>();
                if (settings.Required && !field.Value.HasValue) {
                    updater.AddModelError(field.Name, T("The field {0} is required.", T(field.DisplayName)));
                }
            }

            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, SelectField field, ImportContentContext context) {
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Value", v => field.Value = int.Parse(v));
        }

        protected override void Exporting(ContentPart part, SelectField field, ExportContentContext context) {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Value", field.Value);
        }

        protected override void Describe(DescribeMembersContext context) {
            context
                .Member(null, typeof(Boolean), T("Value"), T("The boolean value of the field."));
        }
    }
}
