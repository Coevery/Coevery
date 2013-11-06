using System.Web.Mvc;
using Coevery.DisplayManagement;
using Coevery.Mvc.Filters;

namespace Coevery.Perspectives.FrontMenu
{
    public class FrontMenuFilter : FilterProvider, IResultFilter {
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly dynamic _shapeFactory;

        public FrontMenuFilter(IWorkContextAccessor workContextAccessor,
            IShapeFactory shapeFactory) {
            _workContextAccessor = workContextAccessor;
            _shapeFactory = shapeFactory;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            // should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult)) {
                return;
            }
            WorkContext workContext = _workContextAccessor.GetContext(filterContext);
            const string menuName = "FrontMenu";
            // Populate main nav
            dynamic menuShape = _shapeFactory.Menu().MenuName(menuName);
            workContext.Layout.Navigation.Add(menuShape);
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {}
    }
}