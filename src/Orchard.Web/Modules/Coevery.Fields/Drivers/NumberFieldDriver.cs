using System;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Coevery.Fields.Fields;
using Coevery.Fields.Settings;
using Orchard.Localization;

namespace Coevery.Fields.Drivers {
    public class NumberFieldDriver : ContentFieldDriver<NumberField> {
        public IOrchardServices Services { get; set; }
        private const string TemplateName = "Fields/Number.Edit";

        public NumberFieldDriver(IOrchardServices services) {
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
                if (field.Value.HasValue) {
                    var intPart = Math.Floor(field.Value.Value);
                    var decPart = field.Value.Value - intPart;
                    if (intPart.ToString().Length > settings.Length) {
                        updater.AddModelError(GetPrefix(field, part), T("The integer part of field {0} is overlength.", T(field.DisplayName)));
                    }
                    if (decPart.ToString().Length > settings.DecimalPlaces + 2) {
                        updater.AddModelError(GetPrefix(field, part), T("The decimal part of field {0} is overlength.", T(field.DisplayName)));
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
