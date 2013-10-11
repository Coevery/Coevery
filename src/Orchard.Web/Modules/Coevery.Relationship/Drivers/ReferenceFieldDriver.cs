using System;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Coevery.Relationship.Fields;
using Coevery.Relationship.Settings;
using Coevery.Relationship.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.Projections.Services;

namespace Coevery.Relationship.Drivers {
    public class StateObject : DynamicObject {
        public override bool TryGetMember(
            GetMemberBinder binder, out object result) {
            result = binder.Name;
            return true;
        }
    }

    public class ReferenceFieldDriver : ContentFieldDriver<ReferenceField> {
        private readonly IContentManager _contentManager;
        private readonly IProjectionManager _projectionManager;
        public IOrchardServices Services { get; set; }
        private const string TemplateName = "Fields/Reference.Edit";

        public ReferenceFieldDriver(
            IOrchardServices services,
            IContentManager contentManager,
            IProjectionManager projectionManager) {
            Services = services;
            _contentManager = contentManager;
            _projectionManager = projectionManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        private static string GetPrefix(ContentField field, ContentPart part) {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(ReferenceField field, ContentPart part) {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, ReferenceField field, string displayType, dynamic shapeHelper) {
            var settings = field.PartFieldDefinition.Settings.GetModel<ReferenceFieldSettings>();
            int? value = field.Value;
            string title = value.HasValue
                ? _contentManager.GetItemMetadata(field.ContentItem).DisplayText
                : string.Empty;

            return ContentShape("Fields_Reference", GetDifferentiator(field, part),
                () => shapeHelper.Fields_Reference(DisplayAsLink: settings.DisplayAsLink, ContentField: field, Title: title));
        }

        protected override DriverResult Editor(ContentPart part, ReferenceField field, dynamic shapeHelper) {
            var settings = field.PartFieldDefinition.Settings.GetModel<ReferenceFieldSettings>();

            var contentItems = _projectionManager.GetContentItems(settings.QueryId)
                .Select(c => new SelectListItem {
                    Text = Services.ContentManager.GetItemMetadata(c).DisplayText,
                    Value = c.Id.ToString(CultureInfo.InvariantCulture),
                    Selected = field.Value == c.Id
                }).ToList();

            var model = new ReferenceFieldViewModel {
                ContentId = field.Value,
                Field = field,
                ItemList = new SelectList(contentItems, "Value", "Text", field.Value)
            };

            return ContentShape("Fields_Reference_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: TemplateName, Model: model, Prefix: GetPrefix(field, part)));
        }

        protected override DriverResult Editor(ContentPart part, ReferenceField field, IUpdateModel updater, dynamic shapeHelper) {
            var viewModel = new ReferenceFieldViewModel();
            if (updater.TryUpdateModel(viewModel, GetPrefix(field, part), null, null)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<ReferenceFieldSettings>();

                if (settings.Required && viewModel.ContentId <= 0) {
                    updater.AddModelError(GetPrefix(field, part), T("The field {0} is mandatory.", T(field.DisplayName)));
                }
                field.Value = viewModel.ContentId;
            }
            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, ReferenceField field, ImportContentContext context) {
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Value", v => field.Value = int.Parse(v), () => field.Value = (int?)null);
        }

        protected override void Exporting(ContentPart part, ReferenceField field, ExportContentContext context) {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Value", field.Value.HasValue ? field.Value.Value.ToString(CultureInfo.InvariantCulture) : String.Empty);
        }

        protected override void Describe(DescribeMembersContext context) {
            context.Member(null, typeof(int?), null, T("The content item id referenced by this field."));
        }
    }
}
