using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Coevery.Logging;
using Coevery.Mvc;
using Coevery.Mvc.Filters;
using IFilterProvider = Coevery.Mvc.Filters.IFilterProvider;

namespace Coevery.Exceptions.Filters {
    public class UnhandledExceptionFilter : FilterProvider, IActionFilter {
        private readonly IExceptionPolicy _exceptionPolicy;
        private readonly ICoeveryServices _coeveryServices;
        private readonly Lazy<IEnumerable<IFilterProvider>> _filterProviders;

        public UnhandledExceptionFilter(
            IExceptionPolicy exceptionPolicy,
            ICoeveryServices coeveryServices,
            Lazy<IEnumerable<IFilterProvider>> filters) {
            _exceptionPolicy = exceptionPolicy;
            _coeveryServices = coeveryServices;
            _filterProviders = filters;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public void OnActionExecuting(ActionExecutingContext filterContext) {
        }

        public void OnActionExecuted(ActionExecutedContext filterContext) {

            // don't provide custom errors if the action has some custom code to handle exceptions
            if(!filterContext.ActionDescriptor.GetCustomAttributes(typeof(HandleErrorAttribute), false).Any()) {
                if (!filterContext.ExceptionHandled && filterContext.Exception != null) {
                    if (_exceptionPolicy.HandleException(this, filterContext.Exception)) {
                        var shape = _coeveryServices.New.ErrorPage();
                        shape.Message = filterContext.Exception.Message;
                        shape.Exception = filterContext.Exception;

                        filterContext.ExceptionHandled = true;

                        // inform exception filters of the exception that was suppressed
                        var exceptionContext = new ExceptionContext(filterContext.Controller.ControllerContext, filterContext.Exception);
                        foreach (var exceptionFilter in _filterProviders.Value.OfType<IExceptionFilter>()) {
                            exceptionFilter.OnException(exceptionContext);
                        }

                        if (exceptionContext.ExceptionHandled) {
                            filterContext.Result = exceptionContext.Result;
                        }
                        else {
                            filterContext.Result = new ShapeResult(filterContext.Controller, shape);
                            filterContext.RequestContext.HttpContext.Response.StatusCode = 500;

                            // prevent IIS 7.0 classic mode from handling the 404/500 itself
                            filterContext.RequestContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                        }
                    }
                }
            }

            if (filterContext.Result is HttpNotFoundResult) {
                var model = _coeveryServices.New.NotFound();
                var request = filterContext.RequestContext.HttpContext.Request;
                var url = request.RawUrl;

                // If the url is relative then replace with Requested path
                model.RequestedUrl = request.Url.OriginalString.Contains(url) & request.Url.OriginalString != url ?
                    request.Url.OriginalString : url;

                // Dont get the user stuck in a 'retry loop' by
                // allowing the Referrer to be the same as the Request
                model.ReferrerUrl = request.UrlReferrer != null &&
                    request.UrlReferrer.OriginalString != model.RequestedUrl ?
                    request.UrlReferrer.OriginalString : null;

                filterContext.Result = new ShapeResult(filterContext.Controller, model);
                filterContext.RequestContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;

                // prevent IIS 7.0 classic mode from handling the 404/500 itself
                filterContext.RequestContext.HttpContext.Response.TrySkipIisCustomErrors = true;
            }
        }
    }
}
