using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Coevery.Mvc.Filters {
    public class CoeveryFilterProvider : System.Web.Mvc.IFilterProvider {

        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor) {
            var workContext = controllerContext.GetWorkContext();
            var filterProviders = workContext.Resolve<IEnumerable<IFilterProvider>>();
            return filterProviders.Select(x => new Filter(x, FilterScope.Action, null));
        }
    }
}