using System.Collections.Generic;
using System.Web.Mvc;
using Coevery.Workflows.Models;
using Coevery.Workflows.Services;

namespace Coevery.Workflows.ViewModels {
    public class AdminEditViewModel {
        public string LocalId { get; set; }
        public bool IsLocal { get; set; }
        public IEnumerable<IActivity> AllActivities { get; set; }
        public WorkflowDefinitionViewModel WorkflowDefinition { get; set; }
        public WorkflowRecord Workflow { get; set; } 
    }

    public class UpdatedActivityModel {
        public string ClientId { get; set; }
        public FormCollection Data { get; set; }

    }
}