using System.Collections.Generic;
using Coevery.Events;

namespace Coevery.Roles.ImportExport {
    public interface ICustomExportStep : IEventHandler {
        void Register(IList<string> steps);
    }

    public class RolesCustomExportStep : ICustomExportStep {
        public void Register(IList<string> steps) {
            steps.Add("Roles");
        }
    }
}