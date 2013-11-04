using System;
using System.Collections.Generic;
using Coevery.Localization;
using Coevery.Tokens;
using Coevery.UI.Notify;
using Coevery.Workflows.Models;
using Coevery.Workflows.Services;

namespace Coevery.Workflows.Activities {
    public class NotificationActivity : Task {
        private readonly INotifier _notifier;
        private readonly ITokenizer _tokenizer;

        public NotificationActivity(INotifier notifier, ITokenizer tokenizer) {
            _notifier = notifier;
            _tokenizer = tokenizer;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public override string Name {
            get { return "Notify"; }
        }

        public override LocalizedString Category {
            get { return T("Notification"); }
        }

        public override LocalizedString Description {
            get { return T("Display a message.");  }
        }

        public override string Form {
            get { return "ActivityNotify"; }
        }

        public override IEnumerable<LocalizedString> GetPossibleOutcomes(WorkflowContext workflowContext, ActivityContext activityContext) {
            yield return T("Done");
        }

        public override IEnumerable<LocalizedString> Execute(WorkflowContext workflowContext, ActivityContext activityContext) {
            var notification = activityContext.GetState<string>("Notification");
            var message = activityContext.GetState<string>("Message");

            NotifyType notificationType;
            Enum.TryParse(notification, true, out notificationType);
            _notifier.Add(notificationType, T(message));

            yield return T("Done");
        }
    }
}