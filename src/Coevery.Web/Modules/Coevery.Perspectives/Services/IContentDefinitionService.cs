using System.Collections.Generic;
using Coevery.Perspectives.ViewModels;

namespace Coevery.Perspectives.Services
{
    public interface IContentDefinitionService : IDependency {
        IEnumerable<EditTypeViewModel> GetUserDefinedTypes();
    }
}