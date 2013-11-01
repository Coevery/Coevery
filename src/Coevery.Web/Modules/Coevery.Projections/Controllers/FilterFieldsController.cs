using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Coevery.Common.Extensions;
using Coevery.Common.ViewModels;
using Coevery.Projections.Services;

namespace Coevery.Projections.Controllers
{
    public class FilterFieldsController : Controller {
        private readonly IProjectionManager _projectionManager;

        public FilterFieldsController(IProjectionManager projectionManager)
        {
            _projectionManager = projectionManager;
        }

        public ActionResult GetFieldFilters(string id) {
            if (string.IsNullOrWhiteSpace(id)) {
                return null;
            }
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            id = pluralService.Singularize(id);

            string category = id.ToPartName() + "ContentFields";
            var filters = _projectionManager.DescribeFilters()
                .Where(x => x.Category == category)
                .SelectMany(x => x.Descriptors);

            var fieldFilters = filters.Select(filter =>
                new FieldFilterViewModel {
                    DisplayName = filter.Name.Text,
                    FormName = filter.Form,
                    Type = filter.Type
                }).ToList();
            return Json(fieldFilters, JsonRequestBehavior.AllowGet);
        }
    }
}