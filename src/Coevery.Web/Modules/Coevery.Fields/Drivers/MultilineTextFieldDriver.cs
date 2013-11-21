using System;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;
using Coevery.ContentManagement.Handlers;
using Coevery.Fields.Fields;
using Coevery.Fields.Settings;
using Coevery.Localization;

namespace Coevery.Fields.Drivers {
    public class MultilineTextFieldDriver : ContentFieldDriver<MultilineTextField> {
        public ICoeveryServices Services { get; set; }
        private const string TemplateName = "Fields/MultilineText.Edit";

        public MultilineTextFieldDriver(ICoeveryServices services) {
            Services = services;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(ContentField field, ContentPart part) {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, MultilineTextField field, string displayType, dynamic shapeHelper) {
            return ContentShape("Fields_MultilineText", GetDifferentiator(field, part), () => {
                var settings = field.PartFieldDefinition.Settings.GetModel<MultilineTextFieldSettings>();
                return shapeHelper.Fields_MultilineText().Settings(settings);
            });
        }

        protected override DriverResult Editor(ContentPart part, MultilineTextField field, dynamic shapeHelper) {
            return ContentShape("Fields_MultilineText_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: field, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, MultilineTextField field, IUpdateModel updater, dynamic shapeHelper) {
            if (updater.TryUpdateModel(field, GetPrefix(field, part), null, null)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<MultilineTextFieldSettings>();

                var hasValue = !string.IsNullOrWhiteSpace(field.Value);
                if (settings.IsUnique && hasValue) {
                    HandleUniqueValue(part, field, updater);
                }
                if (settings.Required && !hasValue) {
                    updater.AddModelError(GetPrefix(field, part), T("The field {0} is mandatory.", T(field.DisplayName)));
                }
                if (hasValue && field.Value.Length > settings.MaxLength) {
                    updater.AddModelError(GetPrefix(field, part), T("The field {0} value exceed max length.", T(field.DisplayName)));
                }
            }
            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, MultilineTextField field, ImportContentContext context) {
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Value", v => field.Value = v);
        }

        protected override void Exporting(ContentPart part, MultilineTextField field, ExportContentContext context) {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Value", field.Value);
        }

        protected override void Describe(DescribeMembersContext context) {
            context
                .Member(null, typeof(string), null, T("The value of the field."));
        }

        private void HandleUniqueValue(ContentPart part, MultilineTextField field, IUpdateModel updater) {
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