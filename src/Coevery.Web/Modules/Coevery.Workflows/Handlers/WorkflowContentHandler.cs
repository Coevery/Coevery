using System.Collections.Generic;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Handlers;
using Coevery.Workflows.Services;

namespace Coevery.Workflows.Handlers {

    public class WorkflowContentHandler : ContentHandler {

        public WorkflowContentHandler(IWorkflowManager workflowManager) {

            OnPublished<ContentPart>(
                (context, part) =>
                    workflowManager.TriggerEvent("ContentPublished",
                    context.ContentItem,
                    () => new Dictionary<string, object> { { "Content", context.ContentItem } }));

            OnRemoving<ContentPart>(
                (context, part) =>
                    workflowManager.TriggerEvent("ContentRemoved",
                    context.ContentItem,
                    () => new Dictionary<string, object> { { "Content", context.ContentItem } }));

            OnVersioned<ContentPart>(
                (context, part1, part2) =>
                    workflowManager.TriggerEvent("ContentVersioned",
                    context.BuildingContentItem,
                    () => new Dictionary<string, object> { { "Content", context.BuildingContentItem } }));

            OnCreated<ContentPart>(
                (context, part) =>
                    workflowManager.TriggerEvent("ContentCreated", context.ContentItem,
                    () => new Dictionary<string, object> { { "Content", context.ContentItem } }));

            OnUpdated<ContentPart>(
                (context, part) =>
                    workflowManager.TriggerEvent("ContentUpdated", context.ContentItem,
                    () => new Dictionary<string, object> { { "Content", context.ContentItem } }));
        }
    }
}