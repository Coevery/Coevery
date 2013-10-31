using System.Collections.Generic;
using Coevery.Events;

namespace Coevery.Workflows.ImportExport {
    public interface ICustomExportStep : IEventHandler {
        void Register(IList<string> steps);
    }

    public class WorkflowsCustomExportStep : ICustomExportStep {
        public void Register(IList<string> steps) {
            steps.Add("Workflows");
        }
    }
}