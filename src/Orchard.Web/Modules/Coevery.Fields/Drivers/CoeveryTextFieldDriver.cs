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
    public class CoeveryTextFieldDriver : ContentFieldDriver<CoeveryTextField> {
        public IOrchardServices Services { get; set; }
        private const string TemplateName = "Fields/CoeveryText.Edit";

        public CoeveryTextFieldDriver(IOrchardServices services) {
            Services = services;
            T = NullLocalizer.Instance;
            DisplayName = "Text";
            Description = "Allows users to enter any combination of letters and numbers.";
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(CoeveryTextField field, ContentPart part) {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, CoeveryTextField field, string displayType, dynamic shapeHelper) {
            return ContentShape("Fields_CoeveryText", GetDifferentiator(field, part), () => {
                var settings = field.PartFieldDefinition.Settings.GetModel<CoeveryTextFieldSettings>();
                return shapeHelper.Fields_Input().Settings(settings);
            });
        }

        protected override DriverResult Editor(ContentPart part, CoeveryTextField field, dynamic shapeHelper) {
            return ContentShape("Fields_CoeveryText_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: field, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, CoeveryTextField field, IUpdateModel updater, dynamic shapeHelper) {
            if (updater.TryUpdateModel(field, GetPrefix(field, part), null, null)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<CoeveryTextFieldSettings>();

                if (settings.Required && string.IsNullOrWhiteSpace(field.Value)) {
                    updater.AddModelError(GetPrefix(field, part), T("The field {0} is mandatory.", T(field.DisplayName)));
                }
            }

            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, CoeveryTextField field, ImportContentContext context) {
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Value", v => field.Value = v);
        }

        protected override void Exporting(ContentPart part, CoeveryTextField field, ExportContentContext context) {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Value", field.Value);
        }

        protected override void Describe(DescribeMembersContext context) {
            context
                .Member(null, typeof(string), T("Value"), T("The value of the field."));
        }
    }
}
