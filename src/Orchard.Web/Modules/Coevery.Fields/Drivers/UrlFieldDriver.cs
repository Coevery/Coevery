using System;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Coevery.Fields.Fields;
using Coevery.Fields.Settings;
using Orchard.Localization;
using Orchard.Utility.Extensions;

namespace Coevery.Fields.Drivers {
    public class UrlFieldDriver : ContentFieldDriver<UrlField> {
        public IOrchardServices Services { get; set; }
        private const string TemplateName = "Fields/Url.Edit";

        public UrlFieldDriver(IOrchardServices services) {
            Services = services;
            T = NullLocalizer.Instance;
            DisplayName = "Url";
            Description = "Allows users to enter Url address.";
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
                return shapeHelper.Fields_Input().Settings(settings);
            });
        }

        protected override DriverResult Editor(ContentPart part, UrlField field, dynamic shapeHelper) {
            //if the content item is new, assign the default value
            if (!part.HasDraft() && !part.HasPublished())
            {
                var settings = field.PartFieldDefinition.Settings.GetModel<UrlFieldSettings>();
                field.Value = settings.DefaultValue;
            }
            return ContentShape("Fields_Url_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: field, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, UrlField field, IUpdateModel updater, dynamic shapeHelper) {
            if (updater.TryUpdateModel(field, GetPrefix(field, part), null, null)) {               
            }
            var settings = field.PartFieldDefinition.Settings.GetModel<UrlFieldSettings>();
            if (settings.Required && string.IsNullOrWhiteSpace(field.Value))
            {
                updater.AddModelError(GetPrefix(field, part), T("The field {0} is mandatory.", T(field.DisplayName)));
            }
            Uri temp;
            if (!Uri.TryCreate(field.Value, UriKind.Absolute, out temp))
            {
                updater.AddModelError(GetPrefix(field, part), T("The field {0} is not valid URL.", T(field.DisplayName)));
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
            context.Member(null, typeof(string), T("Value"), T("The value of the field."));
        }
    }
}
