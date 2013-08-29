using System;
using System.Linq;
using System.Web.Mvc;
using Coevery.OptionSet.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.DisplayManagement;
using Orchard.Events;
using Orchard.Localization;

namespace Coevery.OptionSet.Projections {
    public interface IFormProvider : IEventHandler {
        void Describe(dynamic context);
    }

    public class TermsFilterForms : IFormProvider {
        private readonly IOptionSetService _optionSetService;
        protected dynamic Shape { get; set; }
        public Localizer T { get; set; }

        public TermsFilterForms(
            IShapeFactory shapeFactory,
            IOptionSetService optionSetService) {
            _optionSetService = optionSetService;
            Shape = shapeFactory;
            T = NullLocalizer.Instance;
        }

        public void Describe(dynamic context) {
            Func<IShapeFactory, object> form =
                shape => {

                    var f = Shape.Form(
                        Id: "SelectTerms",
                        _Terms: Shape.SelectList(
                            Id: "termids", Name: "TermIds",
                            Title: T("Terms"),
                            Description: T("Select some terms."),
                            Size: 10,
                            Multiple: true
                            ),
                        _Exclusion: Shape.FieldSet(
                            _OperatorOneOf: Shape.Radio(
                                Id: "operator-is-one-of", Name: "Operator",
                                Title: T("Is one of"), Value: "0", Checked: true
                                ),
                            _OperatorIsAllOf: Shape.Radio(
                                Id: "operator-is-all-of", Name: "Operator",
                                Title: T("Is all of"), Value: "1"
                                )
                            )
                        );

                    foreach (var taxonomy in _optionSetService.GetTaxonomies()) {
                        f._Terms.Add(new SelectListItem { Value = String.Empty, Text = taxonomy.Name });
                        foreach (var term in _optionSetService.GetTerms(taxonomy.Id)) {

                            f._Terms.Add(new SelectListItem { Value = term.Id.ToString(), Text = term.Name });
                        }
                    }

                    return f;
                };

            context.Form("SelectTerms", 
                form, 
                (Action<dynamic, ImportContentContext>) Import, 
                (Action<dynamic, ExportContentContext>) Export
            );
        }

        public void Export(dynamic state, ExportContentContext context) {
            string termIds = Convert.ToString(state.TermIds);

            if (!String.IsNullOrEmpty(termIds)) {
                var ids = termIds.Split(new[] {','}).Select(Int32.Parse).ToArray();
                var terms = ids.Select(_optionSetService.GetOptionItem).ToList();
                var identities = terms.Select(context.ContentManager.GetItemMetadata).Select(x => x.Identity.ToString()).ToArray();

                state.TermIds = String.Join(",", identities);
            }
        }

        public void Import(dynamic state, ImportContentContext context) {
            string termIdentities = Convert.ToString(state.TermIds);

            if (!String.IsNullOrEmpty(termIdentities)) {
                var identities = termIdentities.Split(new[] { ',' }).ToArray();
                var terms = identities.Select(context.GetItemFromSession).ToList();
                var ids = terms.Select(x => x.Id).Select(x => x.ToString()).ToArray();

                state.TermIds = String.Join(",", ids);
            }
        }
    }
}