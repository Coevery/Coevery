using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;
using Coevery.ContentManagement.MetaData;
using Coevery.ContentManagement.Records;
using Coevery.Core.Common.Attributes;
using Coevery.Core.Common.Models;
using Coevery.Core.Common.Services;
using Coevery.Core.Common.ViewModels;
using Coevery.Data;
using JetBrains.Annotations;

namespace Coevery.Core.Common.Drivers {
    [UsedImplicitly]
    public abstract class DynamicRelationshipPartDriver<TPrimaryPart, TContentLinkRecord>
        : ContentPartDriver<TPrimaryPart>
        where TPrimaryPart : ContentPart, new()
        where TContentLinkRecord : ContentLinkRecord, new() {
        private readonly IDynamicRelationshipService _primaryService;
        private readonly IContentManager _contentManager;
        private readonly IRepository<TContentLinkRecord> _contentLinkRepository;
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly string _entityName;
        private readonly bool _isPrimary;

        private const string TemplateName = "Parts/Relationship.Edit";

        protected DynamicRelationshipPartDriver(
            IDynamicRelationshipService primaryService,
            IContentManager contentManager,
            IRepository<TContentLinkRecord> contentLinkRepository,
            IContentDefinitionManager contentDefinitionManager) {
            _primaryService = primaryService;
            _contentManager = contentManager;
            _contentLinkRepository = contentLinkRepository;
            _contentDefinitionManager = contentDefinitionManager;

            var infos = typeof(TPrimaryPart).GetCustomAttributes(typeof(RelationshipInfoAttribute), false);
            if (infos.Length > 0) {
                _entityName = ((RelationshipInfoAttribute) infos[0]).EntityName;
                _isPrimary = ((RelationshipInfoAttribute) infos[0]).IsPrimary;
            }
        }

        private static string GetPrefix(ContentPart part) {
            return part.PartDefinition.Name;
        }

        protected override DriverResult Editor(TPrimaryPart part, dynamic shapeHelper) {
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
                _primaryService.UpdateForContentItem(part.ContentItem, model.SelectedIds, _isPrimary, _contentLinkRepository);
            }

            return Editor(part, shapeHelper);
        }

        private IEnumerable<ContentItemRecord> GetLinks(TPrimaryPart part) {
            if (_isPrimary) {
                return _contentLinkRepository.Table
                    .Where(x => x.PrimaryPartRecord.Id == part.Id)
                    .Select(x => x.RelatedPartRecord);
            }

            return _contentLinkRepository.Table
                .Where(x => x.RelatedPartRecord.Id == part.Id)
                .Select(x => x.PrimaryPartRecord);
        }

        private EditRelationshipViewModel BuildEditorViewModel(TPrimaryPart part) {
            return new EditRelationshipViewModel {
                Links = _primaryService.GetLinks(_entityName).Select(r => new SelectListItem() {
                    Value = r.Id.ToString(),
                    Text = _contentManager.GetItemMetadata(r).DisplayText,
                }).ToList(),
                SelectedIds = GetLinks(part).Select(x => x.Id.ToString()).ToArray(),
                DisplayName = _contentDefinitionManager.GetPartDefinition(typeof(TPrimaryPart).Name).Settings["DisplayName"]
            };
        }
    }
}