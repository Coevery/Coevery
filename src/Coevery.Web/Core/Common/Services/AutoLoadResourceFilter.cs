using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Coevery.DisplayManagement;
using Coevery.Mvc.Filters;

namespace Coevery.Core.Common.Services
{
    public class AutoLoadResourceFilter : FilterProvider, IResultFilter
    {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly dynamic _shapeFactory;
        private readonly IEnumerable<IAutoLoadResourceProvider> _autoLoadResourceProviders;

        public AutoLoadResourceFilter(
            IWorkContextAccessor workContextAccessor,
            IShapeFactory shapeFactory,
            IEnumerable<IAutoLoadResourceProvider> autoLoadResourceProviders)
        {
            _workContextAccessor = workContextAccessor;
            _shapeFactory = shapeFactory;
            _autoLoadResourceProviders = autoLoadResourceProviders;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            // should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult))
                return;

            var ctx = _workContextAccessor.GetContext();
            var footer = ctx.Layout.Footer;
            var resource = _autoLoadResourceProviders.SelectMany<IAutoLoadResourceProvider, dynamic>(x => x.GetResources(_shapeFactory));
            footer.AddRange(resource);
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {
        }
    }
}