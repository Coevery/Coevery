using Orchard;

namespace Coevery.FormDesigner.Services {
    public interface ILayoutManager : IDependency {
        void DeleteField(string entityName, string fieldName);
    }
}