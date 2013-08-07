using Contrib.ContentReference.Fields;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Tokens;

namespace Contrib.ContentReference.Providers {
    [OrchardFeature("Contrib.ContextToken")]
    public class ContextToken : ITokenProvider {
        private readonly IWorkContextAccessor _workContextAccessor;

        private Localizer T { get; set; }

        public ContextToken(
            IWorkContextAccessor workContextAccessor) {
            _workContextAccessor = workContextAccessor;

            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context) {
            context.For("Context", T("Context"), T("The current context"))
                .Token("This", T("Current Content Item"), T("The Content Item represented by the current page"), "Content");
        }

        public void Evaluate(EvaluateContext context) {
            var item = _workContextAccessor.GetContext().GetState<ContentItem>("ContentContext");
            if (item != null) {
                if (!context.Data.ContainsKey("Context")) {
                    context.Data.Add("Context", item);
                }
                context.For<IContent>("Context").Chain("This", "Content", content => item);
            }
        }
    }
}
