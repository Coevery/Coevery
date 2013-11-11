using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Coevery.Common.Extensions;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;

namespace Coevery.Entities.ViewModels {
    public class EditTypeViewModel  {
        public EditTypeViewModel() {
            Settings = new SettingsDictionary();
            Fields = new List<EditPartFieldViewModel>();
            Parts = new List<EditTypePartViewModel>();
        }

        public EditTypeViewModel(ContentTypeDefinition contentTypeDefinition) {
            Name = contentTypeDefinition.Name;
            DisplayName = contentTypeDefinition.DisplayName;
            Settings = contentTypeDefinition.Settings;
            Fields = GetTypeFields(contentTypeDefinition).ToList();
            Parts = GetTypeParts(contentTypeDefinition).ToList();
            _Definition = contentTypeDefinition;
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public SettingsDictionary Settings { get; set; }
        public IEnumerable<EditPartFieldViewModel> Fields { get; set; }
        public IEnumerable<EditTypePartViewModel> Parts { get; set; }
        public IEnumerable<TemplateViewModel> Templates { get; set; }
        public ContentTypeDefinition _Definition { get; private set; }
        public string FieldLabel { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public IEnumerable<SelectListItem> FieldTypes { get; set; }
        public IEnumerable<EntityRecordViewModel> FieldTemplates { get; set; }

        private IEnumerable<EditPartFieldViewModel> GetTypeFields(ContentTypeDefinition contentTypeDefinition) {
            var implicitTypePart = contentTypeDefinition.Parts.SingleOrDefault(p => string.Equals(p.PartDefinition.Name, Name.ToPartName(), StringComparison.OrdinalIgnoreCase));

            return implicitTypePart == null
                ? Enumerable.Empty<EditPartFieldViewModel>()
                : implicitTypePart.PartDefinition.Fields.Select((f, i) => new EditPartFieldViewModel(i, f) { Part = new EditPartViewModel(implicitTypePart.PartDefinition) });
        }

        private IEnumerable<EditTypePartViewModel> GetTypeParts(ContentTypeDefinition contentTypeDefinition) {
            return contentTypeDefinition.Parts
                .Where(p => !string.Equals(p.PartDefinition.Name, Name.ToPartName(), StringComparison.OrdinalIgnoreCase))
                .Select((p, i) => new EditTypePartViewModel(i, p) { Type = this });
        }
    }
}
