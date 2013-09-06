using System;
using Orchard.Core.Common.Models;
using Orchard.DisplayManagement;
using Orchard.Environment.Extensions;
using Orchard.Forms.Services;
using Orchard.Localization;
using Orchard.Projections.Descriptors.Filter;
using Orchard.Tokens;

namespace Piedone.HelpfulLibraries.Libraries.Contents
{
    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public class ContainedByFilter : Orchard.Projections.Services.IFilterProvider
    {
        private readonly ITokenizer _tokenizer;

        public Localizer T { get; set; }

        public ContainedByFilter(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;

            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeFilterContext describe)
        {
            describe.For("Content", T("Content"), T("Content"))
                .Element("ContainedByFilter", T("Contained by filter"), T("Filters for items contained by a container."),
                    ApplyFilter,
                    DisplayFilter,
                    "ContainedByFilter"
                );
        }

        public void ApplyFilter(FilterContext context)
        {
            var id = _tokenizer.Replace(context.State.ContainerId, null, new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });
            context.Query.Where(a => a.ContentPartRecord<CommonPartRecord>(), p => p.Eq("Container.Id", id));
        }

        public LocalizedString DisplayFilter(FilterContext context)
        {
            return T("Content items contained by item with id " + context.State.ContainerId);
        }
    }

    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public class ContentTypesFilterForms : IFormProvider
    {
        private dynamic _shapeFactory { get; set; }

        public Localizer T { get; set; }

        public ContentTypesFilterForms(IShapeFactory shapeFactory)
        {
            _shapeFactory = shapeFactory;

            T = NullLocalizer.Instance;
        }

        public void Describe(Orchard.Forms.Services.DescribeContext context)
        {
            Func<IShapeFactory, object> form =
                shape =>
                {
                    var f = _shapeFactory.Form(
                        Id: "ContainedByFilterForm",
                        _Parts: _shapeFactory.Textbox(
                            Id: "ContainerId", Name: "ContainerId",
                            Title: T("Container Id"),
                            Description: T("The numerical id of the content item containing the items to fetch."),
                            Classes: new[] { "tokenized" })
                        );


                    return f;
                };

            context.Form("ContainedByFilter", form);

        }
    }
}
