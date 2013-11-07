using System.Collections.Generic;
using Coevery.Entities.ViewModels;
using Coevery.ContentManagement.ViewModels;

namespace Coevery.Entities.Services {
    public interface IContentDefinitionService : IDependency {
        IEnumerable<EditTypeViewModel> GetUserDefinedTypes();
        EditTypeViewModel GetType(string name);
        void RemoveType(string name, bool deleteContent);
        IEnumerable<TemplateViewModel> GetFields();
        
    }
}