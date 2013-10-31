using System.Collections.Generic;

namespace Coevery.Reports.Services {
    public interface IReportsPersister : IDependency {
        IEnumerable<Report> Fetch();
        void Save(IEnumerable<Report> reports);
    }
}
