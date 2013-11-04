using System.Linq;
using Coevery.ContentManagement;
using Coevery.Projections.Models;

namespace Coevery.Projections.Services {
    public interface IQueryService : IDependency {
        QueryPart GetQuery(int id);

        QueryPart CreateQuery(string name);
        void DeleteQuery(int id);
    }
}