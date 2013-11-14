using System.Collections.Generic;
using System.Linq;
using Coevery.ContentManagement;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.ContentManagement.MetaData.Models;
using Coevery.ContentManagement.ViewModels;
using Coevery.Events;

namespace Coevery.Entities.Settings {
    public interface IContentDefinitionEditorEvents : IEventHandler {
        IEnumerable<TemplateViewModel> FieldTypeDescriptor();
        void UpdateFieldSettings(string fieldType, string fieldName, SettingsDictionary settingsDictionary, IUpdateModel updateModel);
        void UpdateFieldSettings(ContentPartFieldDefinitionBuilder builder, SettingsDictionary settingsDictionary);
        void FieldDeleted(string fieldType, string fieldName, SettingsDictionary settingsDictionary);

        IEnumerable<TemplateViewModel> TypeEditor(ContentTypeDefinition definition);
        IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition);
        IEnumerable<TemplateViewModel> PartEditor(ContentPartDefinition definition);
        IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition);

        IEnumerable<TemplateViewModel> TypeEditorUpdate(ContentTypeDefinitionBuilder builder, IUpdateModel updateModel);
        IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel);
        IEnumerable<TemplateViewModel> PartEditorUpdate(ContentPartDefinitionBuilder builder, IUpdateModel updateModel);
        IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel);
    }

    public abstract class ContentDefinitionEditorEventsBase : IContentDefinitionEditorEvents {
        public virtual IEnumerable<TemplateViewModel> FieldTypeDescriptor() {
            return Enumerable.Empty<TemplateViewModel>();
        }

        public virtual void UpdateFieldSettings(string fieldType, string fieldName, SettingsDictionary settingsDictionary, IUpdateModel updateModel) {}

        public virtual void UpdateFieldSettings(ContentPartFieldDefinitionBuilder builder, SettingsDictionary settingsDictionary) {}

        public virtual void FieldDeleted(string fieldType, string fieldName, SettingsDictionary settingsDictionary) {}

        public virtual IEnumerable<TemplateViewModel> TypeEditor(ContentTypeDefinition definition) {
            return Enumerable.Empty<TemplateViewModel>();
        }

        public virtual IEnumerable<TemplateViewModel> TypePartEditor(ContentTypePartDefinition definition) {
            return Enumerable.Empty<TemplateViewModel>();
        }

        public virtual IEnumerable<TemplateViewModel> PartEditor(ContentPartDefinition definition) {
            return Enumerable.Empty<TemplateViewModel>();
        }

        public virtual IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            return Enumerable.Empty<TemplateViewModel>();
        }

        public virtual IEnumerable<TemplateViewModel> TypeEditorUpdate(ContentTypeDefinitionBuilder builder, IUpdateModel updateModel) {
            return Enumerable.Empty<TemplateViewModel>();
        }

        public virtual IEnumerable<TemplateViewModel> TypePartEditorUpdate(ContentTypePartDefinitionBuilder builder, IUpdateModel updateModel) {
            return Enumerable.Empty<TemplateViewModel>();
        }

        public virtual IEnumerable<TemplateViewModel> PartEditorUpdate(ContentPartDefinitionBuilder builder, IUpdateModel updateModel) {
            return Enumerable.Empty<TemplateViewModel>();
        }

        public virtual IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            return Enumerable.Empty<TemplateViewModel>();
        }

        protected static TemplateViewModel DefinitionTemplate<TModel>(TModel model) {
            return DefinitionTemplate(model, typeof (TModel).Name, typeof (TModel).Name);
        }

        protected static TemplateViewModel DisplayTemplate<TModel>(TModel model) {
            return DisplayTemplate(model, typeof(TModel).Name, typeof(TModel).Name);
        }

        protected static TemplateViewModel DisplayTemplate<TModel>(TModel model, string templateName, string prefix) {
            return new TemplateViewModel(model, prefix) {
                TemplateName = "DisplayTemplates/" + templateName
            };
        }

        protected static TemplateViewModel DefinitionTemplate<TModel>(TModel model, string templateName, string prefix) {
            return new TemplateViewModel(model, prefix) {
                TemplateName = "DefinitionTemplates/" + templateName
            };
        }
    }
}