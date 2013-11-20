using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coevery.UI.Resources;

namespace Coevery.Core.Common.Services {
    public interface IAutoLoadResourceProvider : IDependency {
        IEnumerable<dynamic> GetResources(dynamic shapeHelper);
    }
}