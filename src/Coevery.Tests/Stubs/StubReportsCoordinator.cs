using Coevery.Reports;
using Coevery.Reports.Services;

namespace Coevery.Tests.Stubs {
    public class StubReportsCoordinator : IReportsCoordinator {
        public void Add(string reportKey, ReportEntryType type, string message) {
            
        }

        public int Register(string reportKey, string activityName, string title) {
            return 0;
        }
    }
}