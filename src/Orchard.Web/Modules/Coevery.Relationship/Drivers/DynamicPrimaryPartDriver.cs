using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Coevery.Relationship.Models;
using Coevery.Relationship.Services;
using Coevery.Relationship.ViewModels;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.Records;
using Orchard.Data;

namespace Coevery.Relationship.Drivers {
    [UsedImplicitly]
    public abstract class DynamicPrimaryPartDriver<TPrimaryPart, TRelatedPart, TPrimaryPartRecord, TRelatedPartRecord, TContentLinkRecord>
        : ContentPartDriver<TPrimaryPart>
        where TPrimaryPart : ContentPart<TPrimaryPartRecord>, new()
        where TRelatedPart : ContentPart<TRelatedPartRecord>
        where TPrimaryPartRecord : ContentPartRecord
        where TRelatedPartRecord : ContentPartRecord
        where TContentLinkRecord : ContentLinkRecord<TPrimaryPartRecord, TRelatedPartRecord>, new() {
        private readonly IDynamicPrimaryService<TPrimaryPart, TRelatedPart, TPrimaryPartRecord, TRelatedPartRecord, TContentLinkRecord> _primaryService;
        private readonly IContentManager _contentManager;
        private readonly IRepository<TContentLinkRecord> _contentLinkRepository;
        private readonly IContentDefinitionManager _contentDefinitionManager;

        protected string _entityName;

        private const string TemplateName = "Parts/Relationship.Edit";

        protected DynamicPrimaryPartDriver(
            IDynamicPrimaryService<TPrimaryPart, TRelatedPart, TPrimaryPartRecord, TRelatedPartRecord, TContentLinkRecord> primaryService,
            IContentManager contentManager,
            IRepository<TContentLinkRecord> contentLinkRepository,
            IContentDefinitionManager contentDefinitionManager) {
            _primaryService = primaryService;
            _contentManager = contentManager;
            _contentLinkRepository = contentLinkRepository;
            _contentDefinitionManager = contentDefinitionManager;
        }

        private static string GetPrefix(ContentPart part) {
            return part.PartDefinition.Name;
        }

        protected override DriverResult Editor(TPrimaryPart part, dynamic shapeHelper) {
            part.Record = part.Record;
            return ContentShape("Parts_Relationship_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: TemplateName,
                    Model: BuildEditorViewModel(part),
                    Prefix: GetPrefix(part)));
        }

        protected override DriverResult Editor(TPrimaryPart part, IUpdateModel updater, dynamic shapeHelper) {
            var model = new EditRelationshipViewModel();
            updater.TryUpdateModel(model, GetPrefix(part), null, null);

            if (part.ContentItem.Id != 0) {
                _primaryService.UpdateForContentItem(part.ContentItem, model.SelectedIds);
            }

            return Editor(part, shapeHelper);
        }

        private IEnumerable<ContentPartRecord> GetLinks(TPrimaryPart part) {
            return _contentLinkRepository.Table
                .Where(x => x.PrimaryPartRecord.Id == part.Id)
                .Select(x => x.RelatedPartRecord);
        }

        private EditRelationshipViewModel BuildEditorViewModel(TPrimaryPart part) {
            return new EditRelationshipViewModel {
                Links = _primaryService.GetLinks(_entityName).Select(r => new SelectListItem() {
                    Value = r.Id.ToString(),
                    Text = _contentManager.GetItemMetadata(_contentManager.Get(r.Id)).DisplayText,
                }).ToList(),
                SelectedIds = GetLinks(part).Select(x => x.Id.ToString()).ToArray(),
                DisplayName = _contentDefinitionManager.GetPartDefinition(typeof (TPrimaryPart).Name).Settings["DisplayName"]
            };
        }
    }
}