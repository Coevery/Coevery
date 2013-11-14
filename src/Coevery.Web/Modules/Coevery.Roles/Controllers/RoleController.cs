using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.ContentManagement;
using Coevery.Localization;
using Coevery.Roles.Services;
using Coevery.Roles.ViewModels;
using Coevery.Security;

namespace Coevery.Roles.Controllers {
    public class RoleController : ApiController {
        private readonly IRoleService _roleService;

        public RoleController(
            ICoeveryServices coeveryServices,
            IRoleService roleService) {
            Services = coeveryServices;
            _roleService = roleService;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ICoeveryServices Services { get; private set; }

        public object Get(int page, int rows) {
            var roles = _roleService.GetRoles();
            var result = roles
                .OrderBy(r => r.Name)
                .Skip((page - 1) * rows)
                .Select(r => new {
                    r.Id,
                    r.Name
                })
                .ToArray();

            var totalRecords = roles.Count();

            return new {
                total = Convert.ToInt32(Math.Ceiling((double) totalRecords / rows)),
                page = page,
                records = totalRecords,
                rows = result
            };
        }

        public HttpResponseMessage Delete(int id) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage roles"))) {
                return Request.CreateErrorResponse(HttpStatusCode.NonAuthoritativeInformation, T("Not authorized to manage roles").Text);
            }

            _roleService.DeleteRole(id);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}