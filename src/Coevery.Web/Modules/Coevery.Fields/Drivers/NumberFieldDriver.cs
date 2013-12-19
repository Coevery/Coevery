using System;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;
using Coevery.ContentManagement.Handlers;
using Coevery.Fields.Fields;
using Coevery.Fields.Settings;
using Coevery.Localization;

namespace Coevery.Fields.Drivers {
    public class NumberFieldDriver : ContentFieldDriver<NumberField> {
        public ICoeveryServices Services { get; set; }
        private const string TemplateName = "Fields/Number.Edit";

        public NumberFieldDriver(ICoeveryServices services) {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(NumberField field, ContentPart part) {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, NumberField field, string displayType, dynamic shapeHelper) {
            return ContentShape("Fields_Number", GetDifferentiator(field, part), () => {
                var settings = field.PartFieldDefinition.Settings.GetModel<NumberFieldSettings>();
                return shapeHelper.Fields_Number().Settings(settings);
            });
        }

        protected override DriverResult Editor(ContentPart part, NumberField field, dynamic shapeHelper) {
            // if the content item is new, assign the default value
            if (!field.Value.HasValue) {
                var settings = field.PartFieldDefinition.Settings.GetModel<NumberFieldSettings>();
                field.Value = settings.DefaultValue;
            }

            return ContentShape("Fields_Number_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: field, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, NumberField field, IUpdateModel updater, dynamic shapeHelper) {
            if (updater.TryUpdateModel(field, GetPrefix(field, part), null, null)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<NumberFieldSettings>();
                if (settings.Required && !field.Value.HasValue) {
                    updater.AddModelError(field.Name, T("The field {0} is required.", T(field.DisplayName)));
                }
                if (field.Value.HasValue && Math.Round(field.Value.Value, settings.DecimalPlaces) != field.Value.Value) {
                    if (settings.DecimalPlaces == 0) {
                        updater.AddModelError(GetPrefix(field, part), T("The field {0} must be an integer", field.DisplayName));
                    }
                    else {
                        updater.AddModelError(GetPrefix(field, part), T("Invalid number of digits for {0}, max allowed: {1}", field.DisplayName, settings.DecimalPlaces));
                    }
                }
            }
            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, NumberField field, ImportContentContext context) {
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Value", v => field.Value = double.Parse(v));
        }

        protected override void Exporting(ContentPart part, NumberField field, ExportContentContext context) {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Value", field.Value);
        }

        protected override void Describe(DescribeMembersContext context) {
            context.Member(null, typeof(double?), null, T("The double value of the field."));
        }
    }
}
