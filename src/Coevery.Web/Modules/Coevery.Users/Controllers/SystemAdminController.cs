using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Coevery.ContentManagement;
using Coevery.Core.Settings.Models;
using Coevery.DisplayManagement;
using Coevery.Localization;
using Coevery.Security;
using Coevery.UI.Notify;
using Coevery.Users.Events;
using Coevery.Users.Models;
using Coevery.Users.Services;
using Coevery.Users.ViewModels;
using Coevery.Mvc.Extensions;
using System;
using Coevery.Settings;
using Coevery.UI.Navigation;
using Coevery.Utility.Extensions;

namespace Coevery.Users.Controllers {
    [ValidateInput(false)]
    public class SystemAdminController : Controller, IUpdateModel {
        private readonly IMembershipService _membershipService;
        private readonly IUserService _userService;
        private readonly IEnumerable<IUserEventHandler> _userEventHandlers;
        private readonly ISiteService _siteService;

        public SystemAdminController(
            ICoeveryServices services,
            IMembershipService membershipService,
            IUserService userService,
            IShapeFactory shapeFactory,
            IEnumerable<IUserEventHandler> userEventHandlers,
            ISiteService siteService) {
            Services = services;
            _membershipService = membershipService;
            _userService = userService;
            _userEventHandlers = userEventHandlers;
            _siteService = siteService;

            T = NullLocalizer.Instance;
            Shape = shapeFactory;
        }

        private dynamic Shape { get; set; }
        public ICoeveryServices Services { get; set; }
        public Localizer T { get; set; }

        public ActionResult List(UserIndexOptions options, PagerParameters pagerParameters) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to list users"))) {
                return new HttpUnauthorizedResult();
            }

            return View();
        }

        public ActionResult Create() {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage users"))) {
                return new HttpUnauthorizedResult();
            }

            var user = Services.ContentManager.New<IUser>("User");
            var editor = Shape.EditorTemplate(TemplateName: "Parts/User.Create", Model: new UserCreateViewModel(), Prefix: null);
            editor.Metadata.Position = "2";
            dynamic model = Services.ContentManager.BuildEditor(user);
            model.Content.Add(editor);

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object) model);
        }

        [HttpPost, ActionName("Create")]
        public ActionResult CreatePOST(UserCreateViewModel createModel) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage users"))) {
                return new HttpUnauthorizedResult();
            }

            if (!string.IsNullOrEmpty(createModel.UserName)) {
                if (!_userService.VerifyUserUnicity(createModel.UserName, createModel.Email)) {
                    AddModelError("NotUniqueUserName", T("User with that username and/or email already exists."));
                }
            }

            if (!Regex.IsMatch(createModel.Email ?? "", UserPart.EmailPattern, RegexOptions.IgnoreCase)) {
                // http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx    
                ModelState.AddModelError("Email", T("You must specify a valid email address."));
            }

            if (createModel.Password != createModel.ConfirmPassword) {
                AddModelError("ConfirmPassword", T("Password confirmation must match"));
            }

            var user = Services.ContentManager.New<IUser>("User");
            if (ModelState.IsValid) {
                user = _membershipService.CreateUser(new CreateUserParams(
                    createModel.UserName,
                    createModel.Password,
                    createModel.Email,
                    null, null, true));
            }

            Services.ContentManager.UpdateEditor(user, this);

            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                return ErrorResult();
            }

            return Json(new {id = user.Id});
        }

        public ActionResult Edit(int id) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage users"))) {
                return new HttpUnauthorizedResult();
            }

            var user = Services.ContentManager.Get<UserPart>(id);
            var editor = Shape.EditorTemplate(TemplateName: "Parts/User.Edit", Model: new UserEditViewModel {User = user}, Prefix: null);
            editor.Metadata.Position = "2";
            dynamic model = Services.ContentManager.BuildEditor(user);
            model.Content.Add(editor);
            model.UserId = id;

            // Casting to avoid invalid (under medium trust) reflection over the protected View method and force a static invocation.
            return View((object) model);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditPOST(int id) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage users"))) {
                return new HttpUnauthorizedResult();
            }

            var user = Services.ContentManager.Get<UserPart>(id, VersionOptions.DraftRequired);
            string previousName = user.UserName;

            Services.ContentManager.UpdateEditor(user, this);

            var editModel = new UserEditViewModel {User = user};
            if (TryUpdateModel(editModel)) {
                if (!_userService.VerifyUserUnicity(id, editModel.UserName, editModel.Email)) {
                    AddModelError("NotUniqueUserName", T("User with that username and/or email already exists."));
                }
                else if (!Regex.IsMatch(editModel.Email ?? "", UserPart.EmailPattern, RegexOptions.IgnoreCase)) {
                    // http://haacked.com/archive/2007/08/21/i-knew-how-to-validate-an-email-address-until-i.aspx    
                    ModelState.AddModelError("Email", T("You must specify a valid email address."));
                }
                else {
                    // also update the Super user if this is the renamed account
                    if (String.Equals(Services.WorkContext.CurrentSite.SuperUser, previousName, StringComparison.Ordinal)) {
                        _siteService.GetSiteSettings().As<SiteSettingsPart>().SuperUser = editModel.UserName;
                    }

                    user.NormalizedUserName = editModel.UserName.ToLowerInvariant();
                }
            }

            if (!ModelState.IsValid) {
                Services.TransactionManager.Cancel();
                return ErrorResult();
            }

            Services.ContentManager.Publish(user.ContentItem);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult SendChallengeEmail(int id) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage users"))) {
                return new HttpUnauthorizedResult();
            }

            var user = Services.ContentManager.Get<IUser>(id);

            if (user != null) {
                var siteUrl = Services.WorkContext.CurrentSite.BaseUrl;
                if (String.IsNullOrWhiteSpace(siteUrl)) {
                    siteUrl = HttpContext.Request.ToRootUrlString();
                }

                _userService.SendChallengeEmail(user.As<UserPart>(), nonce => Url.MakeAbsolute(Url.Action("ChallengeEmail", "Account", new {Area = "Coevery.Users", nonce = nonce}), siteUrl));
                Services.Notifier.Information(T("Challenge email sent to {0}", user.UserName));
            }


            return RedirectToAction("List");
        }

        public ActionResult Approve(int id) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage users"))) {
                return new HttpUnauthorizedResult();
            }

            var user = Services.ContentManager.Get<IUser>(id);

            if (user != null) {
                user.As<UserPart>().RegistrationStatus = UserStatus.Approved;
                Services.Notifier.Information(T("User {0} approved", user.UserName));
                foreach (var userEventHandler in _userEventHandlers) {
                    userEventHandler.Approved(user);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Moderate(int id) {
            if (!Services.Authorizer.Authorize(StandardPermissions.SiteOwner, T("Not authorized to manage users"))) {
                return new HttpUnauthorizedResult();
            }

            var user = Services.ContentManager.Get<IUser>(id);

            if (user != null) {
                if (String.Equals(Services.WorkContext.CurrentUser.UserName, user.UserName, StringComparison.Ordinal)) {
                    Response.StatusCode = (int) HttpStatusCode.MethodNotAllowed;
                    return Content(T("You can't disable your own account. Please log in with another account").Text);
                }
                else {
                    user.As<UserPart>().RegistrationStatus = UserStatus.Pending;
                    Services.Notifier.Information(T("User {0} disabled", user.UserName));
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) {
            return TryUpdateModel(model, prefix, includeProperties, excludeProperties);
        }

        public void AddModelError(string key, LocalizedString errorMessage) {
            ModelState.AddModelError(key, errorMessage.ToString());
        }

        private ContentResult ErrorResult() {
            Response.StatusCode = (int) HttpStatusCode.BadRequest;
            var temp = (from values in ModelState
                from error in values.Value.Errors
                select error.ErrorMessage).ToArray();

            return Content(string.Concat(temp));
        }
    }
}