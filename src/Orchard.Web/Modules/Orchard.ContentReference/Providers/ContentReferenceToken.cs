using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contrib.ContentReference.Fields;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Tokens;

namespace Contrib.ContentReference.Providers {
    [OrchardFeature("Contrib.ContentReference")]
    public class ContentReferenceToken : ITokenProvider {
        private readonly IWorkContextAccessor _workContextAccessor;

        private Localizer T { get; set; }

        public ContentReferenceToken(IWorkContextAccessor workContextAccessor) {
            _workContextAccessor = workContextAccessor;

            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeContext context) {
            context.For("ContentReferenceField", T("Content Reference Field"), T("Tokens for Content Reference Fields"))
                .Token("ContentItem", T("Content Item"), T("The content item referenced."));
        }

        public void Evaluate(EvaluateContext context) {
            context.For<ContentReferenceField>("ContentReferenceField")
                .Chain("ContentItem", "Content", field => field.ContentItem);
        }
    }
}
