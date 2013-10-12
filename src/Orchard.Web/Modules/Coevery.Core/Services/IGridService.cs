using Orchard;

namespace Coevery.Core.Services {
    public interface IGridService : IDependency {
        bool GenerateSortCriteria(string entityName, string sidx, string sord, int queryId);
    }
}