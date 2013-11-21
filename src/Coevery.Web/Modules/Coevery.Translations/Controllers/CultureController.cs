using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Coevery.Translations.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Coevery.Translations.Services;
using Coevery.Localization;

namespace Coevery.Translations.Controllers
{
    public class CultureController : ApiController
    {
        private readonly ILocalizationService _localizationService;

        public CultureController(ILocalizationService localizationService,
            ICoeveryServices orchardServices)
        {
            _localizationService = localizationService;
            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ICoeveryServices Services { get; private set; }

        // GET api/Translations/Culture
        public object Get(string culture, int page, int rows)
        {
            if (!Services.Authorizer.Authorize(Permissions.Translate))
            {
                return new HttpUnauthorizedResult();
            }
            if (culture == null)
            {
                throw new HttpException(404, "Not found");
            }
            new CultureInfo(culture); // Throws if invalid culture
            var model = _localizationService.GetModules(culture);
            model.CanTranslate = Services.Authorizer.Authorize(Permissions.ImportExport) &&
                                 _localizationService.IsCultureAllowed(culture);

            var query = model.Groups.Where(g => g.TotalCount > 0).Select(d => new
            {
                Module = LocalizationHelpers.GetPoFileName(d.Path),
                Culture = model.Culture,
                Path = HttpUtility.UrlEncodeToBytes(d.Path, new UTF8Encoding()),
                Total = d.TotalCount,
                Translated = d.TranslationCount,
                Missing = d.TotalCount - d.TranslationCount
            }).OrderBy(d => d.Module).ThenByDescending(d => d.Missing);

            var totalRecords = model.Groups.Count();

            return new
            {
                total = Convert.ToInt32(Math.Ceiling((double)totalRecords / rows)),
                page = page,
                records = totalRecords,
                rows = query
            };
        }
    }
}