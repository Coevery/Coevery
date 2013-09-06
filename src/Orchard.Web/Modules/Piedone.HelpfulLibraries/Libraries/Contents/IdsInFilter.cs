using System;
using System.Linq;
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
    public class IdsInFilter : Orchard.Projections.Services.IFilterProvider
    {
        private readonly ITokenizer _tokenizer;

        public Localizer T { get; set; }

        public IdsInFilter(ITokenizer tokenizer)
        {
            _tokenizer = tokenizer;

            T = NullLocalizer.Instance;
        }

        public void Describe(DescribeFilterContext describe)
        {
            describe.For("Content", T("Content"), T("Content"))
                .Element("IdsInFilter", T("Ids In filter"), T("Filters for items having the specified ids."),
                    ApplyFilter,
                    DisplayFilter,
                    "IdsInFilter"
                );
        }

        public void ApplyFilter(FilterContext context)
        {
            if (context.State.ContentIds != null)
            {
                var ids = (string)_tokenizer.Replace(context.State.ContentIds, null, new ReplaceOptions { Encoding = ReplaceOptions.NoEncode });
                var idsArray = ids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                idsArray = (from p in idsArray select p.Trim()).ToArray();
                
                context.Query.Where(a => a.ContentPartRecord<CommonPartRecord>(), p => p.In("Id", idsArray));
            }
        }

        public LocalizedString DisplayFilter(FilterContext context)
        {
            return T("Content items having either ids: " + context.State.ContentIds);
        }
    }

    [OrchardFeature("Piedone.HelpfulLibraries.Contents")]
    public class IdsInFilterForms : IFormProvider
    {
        private dynamic _shapeFactory { get; set; }

        public Localizer T { get; set; }

        public IdsInFilterForms(IShapeFactory shapeFactory)
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
                        Id: "IdsInFilterForm",
                        _Parts: _shapeFactory.Textbox(
                            Id: "ContentIds", Name: "ContentIds",
                            Title: T("Contents Ids"),
                            Description: T("A comma-separated list of the ids of contents to match. Items should have CommonPart attached."),
                            Classes: new[] { "tokenized" })
                        );


                    return f;
                };

            context.Form("IdsInFilter", form);

        }
    }
}
