using System;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Coevery.Fields.Fields;
using Coevery.Fields.Settings;
using Orchard.Localization;

namespace Coevery.Fields.Drivers {
    public class CurrencyFieldDriver : ContentFieldDriver<CurrencyField> {
        public IOrchardServices Services { get; set; }
        private const string TemplateName = "Fields/Currency.Edit";

        public CurrencyFieldDriver(IOrchardServices services) {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(CurrencyField field, ContentPart part) {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, CurrencyField field, string displayType, dynamic shapeHelper) {
            return ContentShape("Fields_Currency", GetDifferentiator(field, part), () => {
                var settings = field.PartFieldDefinition.Settings.GetModel<CurrencyFieldSettings>();
                return shapeHelper.Fields_Currency().Settings(settings);
            });
        }

        protected override DriverResult Editor(ContentPart part, CurrencyField field, dynamic shapeHelper) {
            //if the content item is new, assign the default value
            var settings = field.PartFieldDefinition.Settings.GetModel<CurrencyFieldSettings>();
            if (!field.Value.HasValue) {                
                field.Value = settings.DefaultValue;
            }
            else {
                field.Value = Math.Round(field.Value.Value, settings.DecimalPlaces);
            }

            return ContentShape("Fields_Currency_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: field, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, CurrencyField field, IUpdateModel updater, dynamic shapeHelper) {
            if (updater.TryUpdateModel(field, GetPrefix(field, part), null, null)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<CurrencyFieldSettings>();
                if (settings.Required && !field.Value.HasValue) {
                    updater.AddModelError(GetPrefix(field, part), T("The field {0} is required.", T(field.DisplayName)));
                    return Editor(part, field, shapeHelper);
                }
                if (field.Value.HasValue) {
                    field.Value = Math.Round(field.Value.Value,settings.DecimalPlaces);
                    var intPart = decimal.Floor(field.Value.Value);
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

        protected override void Importing(ContentPart part, CurrencyField field, ImportContentContext context) {
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Value", v => field.Value = decimal.Parse(v));
        }

        protected override void Exporting(ContentPart part, CurrencyField field, ExportContentContext context) {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Value", field.Value);
        }

        protected override void Describe(DescribeMembersContext context) {
            context.Member(null, typeof(decimal?), null, T("The decimal value of the field."));
        }
    }
}
