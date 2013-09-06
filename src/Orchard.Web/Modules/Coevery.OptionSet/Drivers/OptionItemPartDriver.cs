using System;
using System.Linq;
using System.Web.Routing;
using Coevery.OptionSet.Models;
using Coevery.OptionSet.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Feeds;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Settings;
using Orchard.UI.Navigation;

namespace Coevery.OptionSet.Drivers {
    public class OptionItemPartDriver : ContentPartDriver<OptionItemPart> {
        private readonly IOptionSetService _optionSetService;
        private readonly ISiteService _siteService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFeedManager _feedManager;
        private readonly IContentManager _contentManager;

        public OptionItemPartDriver(
            IOptionSetService optionSetService,
            ISiteService siteService,
            IHttpContextAccessor httpContextAccessor,
            IFeedManager feedManager,
            IContentManager contentManager) {
            _optionSetService = optionSetService;
            _siteService = siteService;
            _httpContextAccessor = httpContextAccessor;
            _feedManager = feedManager;
            _contentManager = contentManager;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        protected override string Prefix { get { return "OptionItem"; } }

        protected override DriverResult Display(OptionItemPart part, string displayType, dynamic shapeHelper) {
            return Combined(
                ContentShape("Parts_OptionItemPart_Feed", () => {
                    
                    // generates a link to the RSS feed for this term
                    _feedManager.Register(part.Name, "rss", new RouteValueDictionary { { "optionItem", part.Id } });
                    return null;
                }),
                ContentShape("Parts_OptionItemPart", () => {
                    var pagerParameters = new PagerParameters();
                    var httpContext = _httpContextAccessor.Current();
                    if (httpContext != null) {
                        pagerParameters.Page = Convert.ToInt32(httpContext.Request.QueryString["page"]);
                    }
                    
                    var pager = new Pager(_siteService.GetSiteSettings(), pagerParameters);
                    var optionSet = _optionSetService.GetOptionSet(part.OptionSetId);
                    var totalItemCount = 100;

                    // asign Taxonomy and Term to the content item shape (Content) in order to provide 
                    // alternates when those content items are displayed when they are listed on a term
                    var termContentItems = _optionSetService.GetContentItems(part, pager.GetStartIndex(), pager.PageSize)
                        .Select(c => _contentManager.BuildDisplay(c, "Summary").Taxonomy(optionSet).Term(part));

                    var list = shapeHelper.List();

                    list.AddRange(termContentItems);

                    var pagerShape = shapeHelper.Pager(pager)
                            .TotalItemCount(totalItemCount)
                            .Taxonomy(optionSet)
                            .Term(part);

                    return shapeHelper.Parts_TermPart(ContentItems: list, Taxonomy: optionSet, Pager: pagerShape);
                }));
        }

        protected override DriverResult Editor(OptionItemPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Taxonomies_Term_Fields",
                    () => shapeHelper.EditorTemplate(TemplateName: "Parts/Taxonomies.Term.Fields", Model: part, Prefix: Prefix));
        }

        protected override DriverResult Editor(OptionItemPart termPart, IUpdateModel updater, dynamic shapeHelper) {
            if (updater.TryUpdateModel(termPart, Prefix, null, null)) {
                var existing = _optionSetService.GetTermByName(termPart.OptionSetId, termPart.Name);
                if (existing != null && existing.Record != termPart.Record && existing.Container.ContentItem.Record == termPart.Container.ContentItem.Record) {
                    updater.AddModelError("Name", T("The term {0} already exists at this level", termPart.Name));
                }
            }

            return Editor(termPart, shapeHelper);
        }

        protected override void Exporting(OptionItemPart part, ExportContentContext context) {
            context.Element(part.PartDefinition.Name).SetAttributeValue("Selectable", part.Record.Selectable);
            context.Element(part.PartDefinition.Name).SetAttributeValue("Weight", part.Record.Weight);

            var taxonomy = _contentManager.Get(part.Record.OptionSetId);
            var identity = _contentManager.GetItemMetadata(taxonomy).Identity.ToString();
            context.Element(part.PartDefinition.Name).SetAttributeValue("OptionSetId", identity);
        }

        protected override void Importing(OptionItemPart part, ImportContentContext context) {
            part.Record.Selectable = Boolean.Parse(context.Attribute(part.PartDefinition.Name, "Selectable"));
            part.Record.Weight = Int32.Parse(context.Attribute(part.PartDefinition.Name, "Weight"));

            var identity = context.Attribute(part.PartDefinition.Name, "OptionSetId");
            var contentItem = context.GetItemFromSession(identity);
            
            if (contentItem == null) {
                throw new OrchardException(T("Unknown taxonomy: {0}", identity));
            }

            part.Record.OptionSetId = contentItem.Id;
        }
    }
}