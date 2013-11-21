using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Coevery.Translations.Services;
using Coevery.Localization;

namespace Coevery.Translations.Controllers
{
    public class TranslationController : ApiController {
        private readonly ILocalizationService _localizationService;

        public TranslationController(ILocalizationService localizationService,
            ICoeveryServices orchardServices) {
            _localizationService = localizationService;
            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ICoeveryServices Services { get; private set; }

        // GET api/Translations/Translation
        public object Get(int page, int rows) {
            if (!Services.Authorizer.Authorize(Permissions.Translate)) {
                return new HttpUnauthorizedResult();
            }
            var model = _localizationService.GetCultures();

            var query = model.TranslationStates.Select(d => new {
                Culture = d.Key,
                CultureDisplay = d.Key + " - " + new CultureInfo(d.Key).DisplayName,
                Translatable = model.NumberOfStringsInDefaultCulture,
                Translated = d.Value.NumberOfTranslatedStrings,
                Missing = model.NumberOfStringsInDefaultCulture - d.Value.NumberOfTranslatedStrings
            });

            var totalRecords = model.TranslationStates.Count();
            return new {
                total = Convert.ToInt32(Math.Ceiling((double) totalRecords/rows)),
                page = page,
                records = totalRecords,
                rows = query
            };
        }
    }
}