using System.Collections.Generic;
using Coevery.Entities.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace Coevery.Entities.Services {
    public interface IContentDefinitionService : IDependency {
        IEnumerable<EditTypeViewModel> GetTypes();
        IEnumerable<EditTypeViewModel> GetUserDefinedTypes();
        EditTypeViewModel GetType(string name);
        ContentTypeDefinition AddType(string name, string displayName);
        void AlterType(EditTypeViewModel typeViewModel, IUpdateModel updaterModel);
        void AlterField(string name, string fieldName, IUpdateModel updateModel);
        void RemoveType(string name, bool deleteContent);
        void AddPartToType(string partName, string typeName);
        void RemovePartFromType(string partName, string typeName);
        string GenerateContentTypeNameFromDisplayName(string displayName);
        string GenerateFieldNameFromDisplayName(string partName, string displayName);

        EditPartViewModel GetPart(string name);
        EditPartViewModel AddPart(CreatePartViewModel partViewModel);
        void AlterPart(EditPartViewModel partViewModel, IUpdateModel updater);
        void RemovePart(string name);

        IEnumerable<TemplateViewModel> GetFields();
        void AddFieldToPart(string fieldName, string fieldTypeName, string partName);
        void AddFieldToPart(string fieldName, string displayName, string fieldTypeName, string partName);
        void RemoveFieldFromPart(string fieldName, string partName);
    }
}