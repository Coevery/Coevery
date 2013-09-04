using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Relationship.Records;
using NHibernate.Linq;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Shapes;
using Orchard.Environment;
using Orchard.Localization;
using Coevery.Relationship.Services;

namespace Coevery.Relationship.Shapes {
    public class RelationshipShape : IShapeTableProvider {
        private readonly IRelationshipService _relationshipService;

        public RelationshipShape(IRelationshipService relationshipService) {
            _relationshipService = relationshipService;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Discover(ShapeTableBuilder builder) {

            builder.Describe("Relationships_Display")
                .OnCreated(created => {
                    //var relation = created.Shape;
                    //relation.Metadata.Type = "Relationships_Display";
                })
                .OnDisplaying(displaying => {
                    var shape = displaying.Shape;
                    string contentType = shape.ContentType;
                    var relationships = GetRelationships(contentType).ToList();
                    shape.Relationships = relationships;
                });
        }

        private IEnumerable<RelatedEntityViewModel> GetRelationships(string contentType) {
            var records = _relationshipService.GetRelationships(contentType)
                .Where(x => (x.PrimaryEntity.Name == contentType)
                            || ((RelationshipType) x.Type) == RelationshipType.ManyToMany).ToList();
            foreach (var record in records) {
                var relationshipType = (RelationshipType) record.Type;

                if (relationshipType == RelationshipType.OneToMany) {
                    var oneToMany = _relationshipService.GetOneToMany(record.Id);
                    if (oneToMany.ShowRelatedList) {
                        yield return new RelatedEntityViewModel {
                            Label = oneToMany.RelatedListLabel,
                            RelatedEntityName = record.RelatedEntity.Name,
                            ProjectionId = oneToMany.RelatedListProjection.Id
                        };
                    }
                }
                else {
                    var manyToMany = _relationshipService.GetManyToMany(record.Id);
                    var relatedEntityName = record.PrimaryEntity.Name == contentType ? record.RelatedEntity.Name : record.PrimaryEntity.Name;
                    var projectionId = record.PrimaryEntity.Name == contentType ? manyToMany.RelatedListProjection.Id : manyToMany.PrimaryListProjection.Id;
                    var label = record.PrimaryEntity.Name == contentType ? manyToMany.RelatedListLabel : manyToMany.PrimaryListLabel;
                    var showList = record.PrimaryEntity.Name == contentType ? manyToMany.ShowRelatedList : manyToMany.ShowPrimaryList;
                    if (showList) {
                        yield return new RelatedEntityViewModel {
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