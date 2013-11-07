using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.Common.Extensions;
using Coevery.Entities.Extensions;
using Coevery.Entities.ViewModels;
using Coevery;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;
using Coevery.ContentManagement.MetaData;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;
using Coevery.Core.Contents.Extensions;
using Coevery.Localization;
using Coevery.Utility.Extensions;
using IContentDefinitionEditorEvents = Coevery.Entities.Settings.IContentDefinitionEditorEvents;

namespace Coevery.Entities.Services {
    public class ContentDefinitionService : IContentDefinitionService {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IContentDefinitionExtension _contentDefinitionExtension;
        private readonly IContentDefinitionEditorEvents _contentDefinitionEditorEvents;

        public ContentDefinitionService(
            ICoeveryServices services,
            IContentDefinitionManager contentDefinitionManager,
            IContentDefinitionExtension contentDefinitionExtension,
            IContentDefinitionEditorEvents contentDefinitionEditorEvents) {
            Services = services;
            _contentDefinitionManager = contentDefinitionManager;
            _contentDefinitionEditorEvents = contentDefinitionEditorEvents;
            _contentDefinitionExtension = contentDefinitionExtension;
            T = NullLocalizer.Instance;
        }

        public ICoeveryServices Services { get; set; }
        public Localizer T { get; set; }


        public IEnumerable<EditTypeViewModel> GetUserDefinedTypes() {
            return _contentDefinitionExtension.ListUserDefinedTypeDefinitions().Select(ctd => new EditTypeViewModel(ctd)).OrderBy(m => m.DisplayName);
        }

        public EditTypeViewModel GetType(string name) {
            var contentTypeDefinition = _contentDefinitionManager.GetTypeDefinition(name);

            if (contentTypeDefinition == null)
                contentTypeDefinition = new ContentTypeDefinition(name, name);

            var viewModel = new EditTypeViewModel(contentTypeDefinition) {
                Templates = _contentDefinitionEditorEvents.TypeEditor(contentTypeDefinition)
            };

            foreach (var part in viewModel.Parts) {
                part.Templates = _contentDefinitionEditorEvents.TypePartEditor(part._Definition);
                foreach (var field in part.PartDefinition.Fields)
                    field.Templates = _contentDefinitionEditorEvents.PartFieldEditor(field._Definition);
            }

            if (viewModel.Fields.Any()) {
                foreach (var field in viewModel.Fields)
                    field.Templates = _contentDefinitionEditorEvents.PartFieldEditor(field._Definition);
            }
            return viewModel;
        }

        public void RemoveType(string name, bool deleteContent) {

            // first remove all attached parts
            var typeDefinition = _contentDefinitionManager.GetTypeDefinition(name);
            var partDefinitions = typeDefinition.Parts.ToArray();
            foreach (var partDefinition in partDefinitions) {
                RemovePartFromType(partDefinition.PartDefinition.Name, name);

                // delete the part if it's its own part
                if (string.Equals(partDefinition.PartDefinition.Name,name.ToPartName())) {
                    RemovePart(name.ToPartName());
                }
            }

            _contentDefinitionManager.DeleteTypeDefinition(name);

            // delete all content items (but keep versions)
            if (deleteContent) {
                var contentItems = Services.ContentManager.Query(name).List();
                foreach (var contentItem in contentItems) {
                    Services.ContentManager.Remove(contentItem);
                }
            }

        }

        public void RemovePartFromType(string partName, string typeName) {
            _contentDefinitionManager.AlterTypeDefinition(typeName, typeBuilder => typeBuilder.RemovePart(partName));
        }

        public void RemovePart(string name) {
            var partDefinition = _contentDefinitionManager.GetPartDefinition(name);
            var fieldDefinitions = partDefinition.Fields.ToArray();
            foreach (var fieldDefinition in fieldDefinitions) {
                RemoveFieldFromPart(fieldDefinition.Name, name);
            }

            _contentDefinitionManager.DeletePartDefinition(name);
        }

        public IEnumerable<TemplateViewModel> GetFields() {
            return _contentDefinitionEditorEvents.FieldTypeDescriptor();
        }

        public void RemoveFieldFromPart(string fieldName, string partName) {
            _contentDefinitionManager.AlterPartDefinition(partName, typeBuilder => typeBuilder.RemoveField(fieldName));
        }

        public class Updater : IUpdateModel {
            private readonly IUpdateModel _thunk;

            public Updater(IUpdateModel thunk) {
                _thunk = thunk;
            }

            public Func<string, string> _prefix = x => x;

            public bool TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) where TModel : class {
                return _thunk.TryUpdateModel(model, _prefix(prefix), includeProperties, excludeProperties);
            }

            public void AddModelError(string key, LocalizedString errorMessage) {
                _thunk.AddModelError(_prefix(key), errorMessage);
            }
        }
    }
}