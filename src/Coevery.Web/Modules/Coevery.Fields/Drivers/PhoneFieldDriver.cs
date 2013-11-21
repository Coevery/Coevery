using System;
using System.Text.RegularExpressions;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;
using Coevery.ContentManagement.Handlers;
using Coevery.Fields.Fields;
using Coevery.Fields.Settings;
using Coevery.Localization;

namespace Coevery.Fields.Drivers {
    public class PhoneFieldDriver : ContentFieldDriver<PhoneField> {
        public ICoeveryServices Services { get; set; }
        private const string Pattern = @"^\d{8,12}|(((\(\d{3}\))|(\d{3}\-))?(\(0\d{2,3}\)|0\d{2,3}-)?[1-9]\d{6,7})$";
        private const string TemplateName = "Fields/Phone.Edit";

        public PhoneFieldDriver(ICoeveryServices services) {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(PhoneField field, ContentPart part) {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, PhoneField field, string displayType, dynamic shapeHelper) {
            return ContentShape("Fields_Phone", GetDifferentiator(field, part), () => {
                var settings = field.PartFieldDefinition.Settings.GetModel<PhoneFieldSettings>();
                return shapeHelper.Fields_Phone().Settings(settings);
            });
        }

        protected override DriverResult Editor(ContentPart part, PhoneField field, dynamic shapeHelper) {
            //if the content item is new, assign the default value
            if (string.IsNullOrWhiteSpace(field.Value)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<PhoneFieldSettings>();
                field.Value = settings.DefaultValue;
            }
            return ContentShape("Fields_Phone_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: field, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, PhoneField field, IUpdateModel updater, dynamic shapeHelper) {
            if (updater.TryUpdateModel(field, GetPrefix(field, part), null, null)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<PhoneFieldSettings>();

                var hasValue = !string.IsNullOrWhiteSpace(field.Value);
                if (settings.IsUnique && hasValue) {
                    HandleUniqueValue(part, field, updater);
                }
                if (settings.Required && !hasValue) {
                    updater.AddModelError(GetPrefix(field, part), T("The field {0} is mandatory.", T(field.DisplayName)));
                }
                var regex = new Regex(Pattern, RegexOptions.IgnoreCase);
                if (hasValue && !regex.IsMatch(field.Value)) {
                    updater.AddModelError(GetPrefix(field, part), T("The field {0} is not valid phone number.", T(field.DisplayName)));
                }
            }
            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, PhoneField field, ImportContentContext context) {
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Value", v => field.Value = v);
        }

        protected override void Exporting(ContentPart part, PhoneField field, ExportContentContext context) {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Value", field.Value);
        }

        protected override void Describe(DescribeMembersContext context) {
            context.Member(null, typeof(string), null, T("The value of the field."));
        }

        private void HandleUniqueValue(ContentPart part, PhoneField field, IUpdateModel updater) {
            var recordType = part.GetType().GetProperty("Record").PropertyType;
            Action<IAliasFactory> alias = x => x.ContentPartRecord(recordType);
            Action<IHqlExpressionFactory> notCurrentItem = x => x.Not(y => y.Eq("ContentItemRecord", part.Id));
            Action<IHqlExpressionFactory> predicate = x => x.And(notCurrentItem, y => y.Eq(field.Name, field.Value));

            var count = Services.ContentManager.HqlQuery()
                .ForType(part.TypeDefinition.Name)
                .Where(alias, predicate)
                .Count();

            if (count > 0) {
                updater.AddModelError(GetPrefix(field, part), T("The field {0} value must be unique.", T(field.DisplayName)));
            }
        }
    }
}