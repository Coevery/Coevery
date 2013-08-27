using System;
using System.Collections.Generic;
using System.Linq;
using Coevery.Taxonomies.Models;
using Orchard.ContentManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Utility.Extensions;

namespace Coevery.Taxonomies {
    public class Shapes : IShapeTableProvider {

        public void Discover(ShapeTableBuilder builder) {

            builder.Describe("Taxonomy")
                .OnDisplaying(displaying => {
                    var shape = displaying.Shape;
                    var metadata = displaying.ShapeMetadata;
                    TaxonomyPart taxonomy = shape.ContentPart;
                    shape.Classes.Add("taxonomy");
                });

            builder.Describe("TaxonomyItem")
                .OnDisplaying(displaying => {
                    var shape = displaying.Shape;
                    var metadata = displaying.ShapeMetadata;
                    IContent content = shape.Taxonomy;
                    var taxonomy = content.As<TaxonomyPart>();

                    TermPart term = shape.ContentPart;
                });

            builder.Describe("TaxonomyItemLink")
                .OnDisplaying(displaying => {
                    var shape = displaying.Shape;
                    var metadata = displaying.ShapeMetadata;
                    IContent content = shape.Taxonomy;
                    var taxonomy = content.As<TaxonomyPart>();

                    TermPart term = shape.ContentPart;
                });

            

        }
    }
}