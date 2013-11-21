using System;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;
using Coevery.ContentManagement.Handlers;
using Coevery.Fields.Fields;
using Coevery.Fields.Settings;
using Coevery.Localization;

namespace Coevery.Fields.Drivers {
    public class UrlFieldDriver : ContentFieldDriver<UrlField> {
        public ICoeveryServices Services { get; set; }
        private const string TemplateName = "Fields/Url.Edit";

        public UrlFieldDriver(ICoeveryServices services) {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(UrlField field, ContentPart part) {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, UrlField field, string displayType, dynamic shapeHelper) {
            return ContentShape("Fields_Url", GetDifferentiator(field, part), () => {
                var settings = field.PartFieldDefinition.Settings.GetModel<UrlFieldSettings>();
                return shapeHelper.Fields_Url().Settings(settings);
            });
        }

        protected override DriverResult Editor(ContentPart part, UrlField field, dynamic shapeHelper) {
            //if the content item is new, assign the default value
            if (string.IsNullOrWhiteSpace(field.Value)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<UrlFieldSettings>();
                field.Value = settings.DefaultValue;
            }
            return ContentShape("Fields_Url_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: field, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, UrlField field, IUpdateModel updater, dynamic shapeHelper) {
            if (updater.TryUpdateModel(field, GetPrefix(field, part), null, null)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<UrlFieldSettings>();

                bool hasValue = !string.IsNullOrWhiteSpace(field.Value);
                if (settings.IsUnique && hasValue) {
                    HandleUniqueValue(part, field, updater);
                }
                if (settings.Required && !hasValue) {
                    updater.AddModelError(GetPrefix(field, part), T("The field {0} is mandatory.", T(field.DisplayName)));
                }
                Uri temp;
                if (hasValue && !Uri.TryCreate(field.Value, UriKind.Absolute, out temp)) {
                    updater.AddModelError(GetPrefix(field, part), T("The field {0} is not valid URL.", T(field.DisplayName)));
                }
            }
            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, UrlField field, ImportContentContext context) {
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Value", v => field.Value = v);
        }

        protected override void Exporting(ContentPart part, UrlField field, ExportContentContext context) {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Value", field.Value);
        }

        protected override void Describe(DescribeMembersContext context) {
            context.Member(null, typeof(string), null, T("The value of the field."));
        }


        private void HandleUniqueValue(ContentPart part, UrlField field, IUpdateModel updater) {
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