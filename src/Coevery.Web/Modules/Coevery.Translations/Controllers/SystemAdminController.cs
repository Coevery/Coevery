using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Coevery.Translations.Helpers;
using Coevery.Translations.Services;
using Coevery.Logging;
using Coevery.Localization;
using Coevery.Data;
using Coevery.Translations.Models;
using Coevery.UI.Notify;
using Coevery.Translations.ViewModels;

namespace Coevery.Translations.Controllers
{
    public class SystemAdminController : Controller
    {
        private readonly ILocalizationService _localizationService;

        public ILogger Logger { get; set; }
        public Localizer T { get; set; }
        public ICoeveryServices Services { get; set; }
        public ILocalizationManagementService ManagementService { get; set; }
        public IRepository<LocalizableStringRecord> Repo { get; set; }

        public SystemAdminController(
          ILocalizationService localizationService,
          ICoeveryServices services,
          ILocalizationManagementService managementService,
          IRepository<LocalizableStringRecord> repo)
        {
            _localizationService = localizationService;
            Services = services;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            ManagementService = managementService;
            Repo = repo;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Culture(string id)
        {
            ViewData["Culture"] = id;
            return View();
        }

        public ActionResult Detail(string id, byte[] path)
        {
            var pathTemp = HttpUtility.UrlDecode(path, new UTF8Encoding());
            if (!Services.Authorizer.Authorize(Permissions.Translate))
            {
                return new HttpUnauthorizedResult();
            }
            if (id == null)
            {
                throw new HttpException(404, "Not found");
            }
            new CultureInfo(id); // Throws if invalid culture
            var model = _localizationService.GetTranslations(id, pathTemp);
            model.CurrentGroupPath = pathTemp;
            model.CanTranslate = Services.Authorizer.Authorize(Permissions.ImportExport) &&
                                 _localizationService.IsCultureAllowed(id);
            ViewData["ModuleName"] = LocalizationHelpers.GetPoFileName(pathTemp);
            return View(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Detail(string culture)
        {
            if (!Services.Authorizer.Authorize(
                Permissions.Translate, T("You are not allowed to update translations.")))
            {
                return new HttpUnauthorizedResult();
            }

            if (!_localizationService.IsCultureAllowed(culture))
            {
                return new HttpUnauthorizedResult();
            }

            var paramsTemp = HttpContext.Request.Params;
            var viewModel = paramsTemp.AllKeys.Where(d => d != null && Regex.IsMatch(d, "^\\d+$")).Select(para => new CultureGroupDetailsViewModel.TranslationViewModel
            {
                Id = Convert.ToInt32(para),
                LocalString = paramsTemp[para]
            }).ToList();

            foreach (var translationViewModel in viewModel)
            {
                _localizationService.UpdateTranslation(translationViewModel.Id, culture, translationViewModel.LocalString);
            }

            _localizationService.ResetCache();

            return Content("Success");
        }

        public ActionResult Search()
        {
            var cultures = _localizationService.GetCultures();
            return View(cultures);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Search(string culture, string queryString)
        {
            if (!string.IsNullOrEmpty(queryString))
            {
                var model = _localizationService.Search(culture, queryString);
                model.CurrentGroupPath = queryString;
                model.CanTranslate = Services.Authorizer.Authorize(Coevery.Translations.Permissions.ImportExport) &&
                                     _localizationService.IsCultureAllowed(culture);
                return PartialView("SearchDetail", model);
            }
            return Content("Success");
        }

        public ActionResult Import()
        {
            var culture = _localizationService.GetCultures();
            return View(culture);
        }

        public ActionResult FromSource(string culture)
        {
            if (!Services.Authorizer.Authorize(Permissions.ImportExport))
            {
                return new HttpUnauthorizedResult();
            }
            var translations = ManagementService.ExtractDefaultTranslation(Server.MapPath("~"),null).ToList();

            _localizationService.SaveStringsToDatabase(translations, false);

            Services.Notifier.Add(NotifyType.Information, T("Imported {0} translatable strings", translations.Count));

            if (!string.IsNullOrEmpty(culture))
            {
                ImportLiveOrchardPo(culture);
            }

            _localizationService.ResetCache();
            return Content("Success");
        }

        public ActionResult ImportLiveOrchardPo(string culture)
        {
            if (!Services.Authorizer.Authorize(Permissions.ImportExport))
            {
                return new HttpUnauthorizedResult();
            }
            IEnumerable<StringEntry> strings;
            var url = "http://www.orchardproject.net/Localize/download/" + culture;
            var req = WebRequest.Create(url);
            using (var stream = req.GetResponse().GetResponseStream())
            {
                strings = _localizationService.GetTranslationsFromZip(stream).ToList();
            }
            _localizationService.SaveStringsToDatabase(strings, false);
            _localizationService.ResetCache();
            return Content("Success");
        }

        [HttpPost]
        public ActionResult ManualAdd(LocalizableStringRecord record, string culture, string translationResult)
        {
            if (!Services.Authorizer.Authorize(Permissions.Translate))
            {
                return new HttpUnauthorizedResult();
            }
            if (String.IsNullOrWhiteSpace(record.Context) ||
                String.IsNullOrWhiteSpace(record.StringKey) ||
                String.IsNullOrWhiteSpace(record.Path))
            {
                Services.Notifier.Add(NotifyType.Error, T("Missing required information"));
            }
            else
            {
                record.OriginalLanguageString = record.StringKey;
                Repo.Create(record);
                Services.Notifier.Add(NotifyType.Information, T("Added 1 translation: {0}", record.OriginalLanguageString));

                _localizationService.UpdateTranslation(record.Id, culture, translationResult);
            }
            return Content("Success");
        }
    }
}