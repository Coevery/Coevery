using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Coevery.ContentManagement;
using Coevery.Localization;
using Coevery.Security;
using Coevery.Users.Models;

namespace Coevery.Users.Controllers {
    public class UserController : ApiController {
        public UserController(ICoeveryServices coeveryServices) {
            Services = coeveryServices;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }
        public ICoeveryServices Services { get; private set; }

        public object Get(int page, int rows) {
            var users = Services.ContentManager
                .Query<UserPart, UserPartRecord>()
                .List().Select(x => new {
                    x.Id,
                    x.UserName,
                    x.Email,
                    RegistrationStatus = T(x.RegistrationStatus.ToString()).Text
                }).ToArray();

            var totalRecords = users.Count();
            return new {
                total = Convert.ToInt32(Math.Ceiling((double) totalRecords / rows)),
                page = page,
                records = totalRecords,
                rows = users
            };
        }

        public HttpResponseMessage Delete(int id) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage users"))) {
                return Request.CreateErrorResponse(HttpStatusCode.NonAuthoritativeInformation, T("Not authorized to manage users").Text);
            }

            var user = Services.ContentManager.Get<IUser>(id);
            string message = null;

            if (user != null) {
                if (String.Equals(Services.WorkContext.CurrentSite.SuperUser, user.UserName, StringComparison.Ordinal)) {
                    message = T("The Super user can't be removed. Please disable this account or specify another Super user account").Text;
                }
                else if (String.Equals(Services.WorkContext.CurrentUser.UserName, user.UserName, StringComparison.Ordinal)) {
                    message = T("You can't remove your own account. Please log in with another account").Text;
                }
                else {
                    Services.ContentManager.Remove(user.ContentItem);
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }

            return Request.CreateResponse(HttpStatusCode.MethodNotAllowed, message);
        }
    }
}