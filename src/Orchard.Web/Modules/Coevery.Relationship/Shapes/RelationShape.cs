using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Environment;
using Orchard.Localization;
using Coevery.Relationship.Services;

using Orchard.Utility.Extensions;

// ReSharper disable InconsistentNaming

namespace Coevery.Relationship.Shapes {
    public class RelationShape : IShapeTableProvider {
        private readonly IRelationshipService _relationshipService;
        private readonly Work<IShapeFactory> _shapeFactory;

        public RelationShape(
            IRelationshipService relationshipService,
            Work<IShapeFactory> shapeFactory
            ) {
            _relationshipService = relationshipService;
            _shapeFactory = shapeFactory;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void Discover(ShapeTableBuilder builder) {

            builder.Describe("Relationships_Display")
                .OnCreated(created => {
                    var relation = created.Shape;
                    relation.Metadata.Type = "Relationships_Display";
                })
                .OnDisplaying(displaying => {
                    var relations = displaying.Shape;
                    var records = _relationshipService.GetRelationships((string)relations["EntityName"]);
                    relations["Relations"] = (from record in records
                                              select record.Name).ToArray();
                });
        }

        //[Shape]
        //public IHtmlString Relationships_Display(dynamic Shape, dynamic Display,string entityName) {
        //    foreach (var record in _relationshipService.GetRelationships(entityName)) {
        //        Shape.Attributes.Add(record.Id.ToString("D"), record.Name);
        //    }
        //    Shape.Metadata.Type = "RelationshipView";
        //    return Display(Shape);
        //}
    }
}