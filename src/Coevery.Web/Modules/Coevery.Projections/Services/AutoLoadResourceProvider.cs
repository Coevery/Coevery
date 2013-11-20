using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.Core.Common.Services;

namespace Coevery.Projections.Services {
    public class AutoLoadResourceProvider : IAutoLoadResourceProvider {
        public IEnumerable<dynamic> GetResources(dynamic shapeHelper) {
            yield return shapeHelper.jqGrid_Script();
        }
    }
}