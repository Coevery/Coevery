using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard;
using Orchard.DisplayManagement;
using Orchard.Mvc.Filters;
using Orchard.UI.Navigation;

namespace Coevery.Core.Admin {
    public class SystemMenuFilter : FilterProvider, IResultFilter {
        private readonly INavigationManager _navigationManager; 
        private readonly IWorkContextAccessor _workContextAccessor;
        private readonly dynamic _shapeFactory;

        public SystemMenuFilter(INavigationManager navigationManager,
            IWorkContextAccessor workContextAccessor,
            IShapeFactory shapeFactory) {

            _navigationManager = navigationManager;
            _workContextAccessor = workContextAccessor;
            _shapeFactory = shapeFactory;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            // should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult)) {
                return;
            }

            WorkContext workContext = _workContextAccessor.GetContext(filterContext);

            const string menuName = "SystemAdmin";
            if (!SystemAdminFilter.IsApplied(filterContext.RequestContext)) {
                return;
            }

            IEnumerable<MenuItem> menuItems = _navigationManager.BuildMenu(menuName);

            // adding query string parameters
            var routeData = new RouteValueDictionary(filterContext.RouteData.Values);
            var queryString = workContext.HttpContext.Request.QueryString;
            if (queryString != null) {
                foreach (var key in from string key in queryString.Keys where key != null && !routeData.ContainsKey(key) let value = queryString[key] select key) {
                    routeData[key] = queryString[key];
                }
            }

            var request = _workContextAccessor.GetContext().HttpContext.Request;

            // Set the currently selected path
            Stack<MenuItem> selectedPath = NavigationHelper.SetSelectedPath(menuItems, request, routeData);

            // Populate main nav
            dynamic menuShape = _shapeFactory.Menu().MenuName(menuName);
            NavigationHelper.PopulateMenu(_shapeFactory, menuShape, menuShape, menuItems);

            // Add any know image sets to the main nav
            IEnumerable<string> menuImageSets = _navigationManager.BuildImageSets(menuName);
            if (menuImageSets != null && menuImageSets.Any())
                menuShape.ImageSets(menuImageSets);

            workContext.Layout.Navigation.Add(menuShape);

            // Populate local nav
            dynamic localMenuShape = _shapeFactory.LocalMenu().MenuName(string.Format("local_{0}", menuName));
            NavigationHelper.PopulateLocalMenu(_shapeFactory, localMenuShape, localMenuShape, selectedPath);
            workContext.Layout.LocalNavigation.Add(localMenuShape);
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) { }
    }
}