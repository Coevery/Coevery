using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.Localization;
using Coevery.Scripting.CSharp.Services;
using Coevery.Workflows.Models;
using Coevery.Workflows.Services;

namespace Coevery.Scripting.CSharp.Activities {
    public class DecisionActivity : Task {
        private readonly ICSharpService _csharpService;
        private readonly ICoeveryServices _coeveryServices;
        private readonly IWorkContextAccessor _workContextAccessor;

        public DecisionActivity(
            ICoeveryServices coeveryServices,
            ICSharpService csharpService,
            IWorkContextAccessor workContextAccessor) {
            _csharpService = csharpService;
            _coeveryServices = coeveryServices;
            _workContextAccessor = workContextAccessor;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public override string Name {
            get { return "Decision"; }
        }

        public override LocalizedString Category {
            get { return T("Misc"); }
        }

        public override LocalizedString Description {
            get { return T("Evaluates an expression."); }
        }

        public override string Form {
            get { return "ActivityActionDecision"; }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext) {
            return GetOutcomes(activityContext).Select(outcome => T(outcome));
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext) {
            var script = activityContext.GetState<string>("Script");
            object outcome = null;

            _csharpService.SetParameter("Services", _coeveryServices);
            _csharpService.SetParameter("ContentItem", (dynamic)workflowContext.Content.ContentItem);
            _csharpService.SetParameter("WorkContext", _workContextAccessor.GetContext());
            _csharpService.SetFunction("T", (Func<string, string>)(x => T(x).Text));
            _csharpService.SetFunction("SetOutcome", (Action<object>)(x => outcome = x));

            _csharpService.Run(script);

            yield return T(Convert.ToString(outcome));
        }

        private IEnumerable<string> GetOutcomes(ActivityContext context) {

            var outcomes = context.GetState<string>("Outcomes");

            if (String.IsNullOrEmpty(outcomes)) {
                return Enumerable.Empty<string>();
            }

            return outcomes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

        }
    }
}