using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.Common.Services;

namespace Coevery.Projections.Services {
    public class JavascriptLocalizationResourceProvider : IAutoLoadResourceProvider {
        public IEnumerable<dynamic> GetResources(dynamic shapeHelper) {
            yield return shapeHelper.Projection_JavascriptLocalizationResource();
        }
    }
}