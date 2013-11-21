using System;
using System.Text.RegularExpressions;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;
using Coevery.ContentManagement.Handlers;
using Coevery.Fields.Fields;
using Coevery.Fields.Settings;
using Coevery.Localization;

namespace Coevery.Fields.Drivers {
    public class EmailFieldDriver : ContentFieldDriver<EmailField> {
        public ICoeveryServices Services { get; set; }
        private const string TemplateName = "Fields/Email.Edit";

        private const string Pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                                       + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                                       + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

        public EmailFieldDriver(ICoeveryServices services) {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(EmailField field, ContentPart part) {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, EmailField field, string displayType, dynamic shapeHelper) {
            return ContentShape("Fields_Email", GetDifferentiator(field, part), () => {
                var settings = field.PartFieldDefinition.Settings.GetModel<EmailFieldSettings>();
                return shapeHelper.Fields_Email().Settings(settings);
            });
        }

        protected override DriverResult Editor(ContentPart part, EmailField field, dynamic shapeHelper) {
            if (string.IsNullOrWhiteSpace(field.Value)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<EmailFieldSettings>();
                field.Value = settings.DefaultValue;
            }
            return ContentShape("Fields_Email_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: field, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, EmailField field, IUpdateModel updater, dynamic shapeHelper) {
            if (updater.TryUpdateModel(field, GetPrefix(field, part), null, null)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<EmailFieldSettings>();

                var hasValue = !string.IsNullOrWhiteSpace(field.Value);
                if (settings.IsUnique && hasValue) {
                    HandleUniqueValue(part, field, updater);
                }
                if (settings.Required && !hasValue) {
                    updater.AddModelError(GetPrefix(field, part), T("The field {0} is mandatory.", T(field.DisplayName)));
                }
                var regex = new Regex(Pattern, RegexOptions.IgnoreCase);
                if (hasValue && !regex.IsMatch(field.Value)) {
                    updater.AddModelError(GetPrefix(field, part), T("The field {0} is not valid Email.", T(field.DisplayName)));
                }
            }
            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, EmailField field, ImportContentContext context) {
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Value", v => field.Value = v);
        }

        protected override void Exporting(ContentPart part, EmailField field, ExportContentContext context) {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Value", field.Value);
        }

        protected override void Describe(DescribeMembersContext context) {
            context.Member(null, typeof(string), null, T("The value of the field."));
        }

        private void HandleUniqueValue(ContentPart part, EmailField field, IUpdateModel updater) {
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