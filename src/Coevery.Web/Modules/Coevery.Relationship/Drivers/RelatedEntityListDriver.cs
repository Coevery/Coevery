using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using Coevery.Entities.Services;
using Coevery.Relationship.Records;
using Coevery.Relationship.Services;
using Coevery.Relationship.ViewModels;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Drivers;

namespace Coevery.Relationship.Drivers {
    public class RelatedEntityListDriver : ContentPartDriver<ContentPart> {
        private readonly IRelationshipService _relationshipService;
        private readonly ISettingService _settingService;

        public RelatedEntityListDriver(
            IRelationshipService relationshipService,
            ISettingService settingService) {
            _settingService = settingService;
            _relationshipService = relationshipService;
        }

        protected override DriverResult Display(ContentPart part, string displayType, dynamic shapeHelper) {
            var dynamicParts = part.ContentItem.Parts.Where(x => x.GetType().Namespace == "Coevery.DynamicTypes.Records").ToList();
            if (dynamicParts.Any()) {
                string contentType = part.ContentItem.ContentType;
                var relationships = GetRelationships(contentType).ToList();
                if (relationships.Any())
                    return ContentShape("RelatedEntityList",
                        () => shapeHelper.RelatedEntityList(Relationships: relationships));
            }

            return null;
        }

        private IEnumerable<RelatedEntityViewModel> GetRelationships(string contentType) {
            var records = _relationshipService.GetRelationships(contentType)
                .Where(x => (x.PrimaryEntity.Name == contentType)
                            || ((RelationshipType)x.Type) == RelationshipType.ManyToMany).ToList();

            foreach (var record in records) {
                var relationshipType = (RelationshipType)record.Type;

                if (relationshipType == RelationshipType.OneToMany) {
                    var oneToMany = _relationshipService.GetOneToMany(record.Id);
                    if (oneToMany.ShowRelatedList) {
                        yield return new RelatedEntityViewModel {
                            RelationId = _relationshipService.GetReferenceField(record.RelatedEntity.Name, record.Name),
                            RelationType = "OneToMany",
                            Label = oneToMany.RelatedListLabel,
                            RelatedEntityName = _settingService.ParseSetting(record.RelatedEntity.Settings)["CollectionName"],
                            ProjectionId = oneToMany.RelatedListProjection.Id
                        };
                    }
                }
                else {
                    var manyToMany = _relationshipService.GetManyToMany(record.Id);
                    var relatedEntityName = record.PrimaryEntity.Name == contentType
                        ? _settingService.ParseSetting(record.RelatedEntity.Settings)["CollectionName"]
                        : _settingService.ParseSetting(record.PrimaryEntity.Settings)["CollectionName"];
                    var projectionId = record.PrimaryEntity.Name == contentType ? manyToMany.RelatedListProjection.Id : manyToMany.PrimaryListProjection.Id;
                    var label = record.PrimaryEntity.Name == contentType ? manyToMany.RelatedListLabel : manyToMany.PrimaryListLabel;
                    var showList = record.PrimaryEntity.Name == contentType ? manyToMany.ShowRelatedList : manyToMany.ShowPrimaryList;
                    if (showList) {
                        yield return new RelatedEntityViewModel {
                            RelationId = record.Name,
                            RelationType = "ManyToMany",
                            Label = label,
                            RelatedEntityName = relatedEntityName,
                            ProjectionId = projectionId
                        };
                    }
                }
            }
        }
    }
}