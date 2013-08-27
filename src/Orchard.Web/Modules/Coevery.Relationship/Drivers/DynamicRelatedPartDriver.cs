using System.Linq;
using Coevery.Relationship.Models;
using Coevery.Relationship.Services;
using Coevery.Relationship.ViewModels;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;

namespace Coevery.Relationship.Drivers {
    [UsedImplicitly]
    public abstract class DynamicRelatedPartDriver<TPrimaryPart, TRelatedPart, TPrimaryPartRecord, TRelatedPartRecord, TContentLinkRecord>
        : ContentPartDriver<TRelatedPart>
        where TPrimaryPart : DynamicPrimaryPart<TPrimaryPartRecord, TContentLinkRecord>
        where TRelatedPart : DynamicPrimaryPart<TRelatedPartRecord, TContentLinkRecord>, new()
        where TPrimaryPartRecord : DynamicPartRecord<TContentLinkRecord>
        where TRelatedPartRecord : DynamicPartRecord<TContentLinkRecord>
        where TContentLinkRecord : IContentLinkRecord, new() {
        private readonly IDynamicRelatedService<TPrimaryPart, TRelatedPart, TPrimaryPartRecord, TRelatedPartRecord, TContentLinkRecord> _relatedService;
        private readonly IContentManager _contentManager;

        private const string TemplateName = "Parts/Primary";

        protected DynamicRelatedPartDriver(
            IDynamicRelatedService<TPrimaryPart, TRelatedPart, TPrimaryPartRecord, TRelatedPartRecord, TContentLinkRecord> relatedService,
            IContentManager contentManager) {
            _relatedService = relatedService;
            _contentManager = contentManager;
        }

        protected override string Prefix {
            get { return "Primary"; }
        }

        protected override DriverResult Display(TRelatedPart part, string displayType, dynamic shapeHelper) {
            return ContentShape("Parts_Primary",
                            () => shapeHelper.Parts_Primary(
                                ContentPart: part,
                                Titles: part.Links.Select(x => _contentManager.GetItemMetadata(_contentManager.Get(x.Id)).DisplayText)));
        }

        protected override DriverResult Editor(TRelatedPart part, dynamic shapeHelper) {
            return ContentShape("Parts_Rewards_Edit",
                    () => shapeHelper.EditorTemplate(
                        TemplateName: TemplateName,
                        Model: BuildEditorViewModel(part),
                        Prefix: Prefix));
        }

        protected override DriverResult Editor(TRelatedPart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new EditRelationshipViewModel();
            updater.TryUpdateModel(model, Prefix, null, null);

            if (part.ContentItem.Id != 0) {
                _relatedService.UpdateForContentItem(part.ContentItem, model.Links);
            }

            return Editor(part, shapeHelper);
        }

        private EditRelationshipViewModel BuildEditorViewModel(TRelatedPart part) {
            var itemRewards = part.Links.ToLookup(r => r.Id);
            return new EditRelationshipViewModel {
                Links = _relatedService.GetLinks().Select(r => new RelationshipEntry() {
                    Id = r.Id,
                    Title = _contentManager.GetItemMetadata(_contentManager.Get(r.Id)).DisplayText,
                    IsChecked = itemRewards.Contains(r.Id)
                }).ToList()
            };
        }
    }
}