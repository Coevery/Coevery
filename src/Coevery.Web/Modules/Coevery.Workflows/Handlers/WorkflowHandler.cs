using System.Linq;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Handlers;
using Coevery.Data;
using Coevery.Workflows.Models;

namespace Coevery.Workflows.Handlers {

    public class WorkflowHandler : ContentHandler {

        public WorkflowHandler(
            IRepository<WorkflowRecord> workflowRepository
            ) {

            // Delete any pending workflow related to a deleted content item
            OnRemoving<ContentPart>(
                (context, part) => {
                    var workflows = workflowRepository.Table.Where(x => x.ContentItemRecord == context.ContentItemRecord).ToList();

                    foreach (var item in workflows) {
                        workflowRepository.Delete(item);
                    }
                });
        }
    }
}