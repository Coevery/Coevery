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
    public class TranslationController : ApiController
    {
        private readonly ILocalizationService _localizationService;

        public TranslationController(ILocalizationService localizationService,
            ICoeveryServices orchardServices)
        {
            _localizationService = localizationService;
            Services = orchardServices;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public ICoeveryServices Services { get; private set; }

        // GET api/Translations/Translation
        public object Get(int page, int rows)
        {
            if (!Services.Authorizer.Authorize(Permissions.Translate))
            {
                return new HttpUnauthorizedResult();
            }
            var model = _localizationService.GetCultures();

            var query = model.TranslationStates.Select(d => new
            {
                Culture = d.Key,
                CultureDisplay = d.Key + " - " + new CultureInfo(d.Key).DisplayName,
                Translatable = model.NumberOfStringsInDefaultCulture,
                Translated = d.Value.NumberOfTranslatedStrings,
                Missing = model.NumberOfStringsInDefaultCulture - d.Value.NumberOfTranslatedStrings
            });

            var totalRecords = model.TranslationStates.Count();
            return new
            {
                total = Convert.ToInt32(Math.Ceiling((double)totalRecords / rows)),
                page = page,
                records = totalRecords,
                rows = query
            };
        }

        public HttpResponseMessage GetCultureColumns()
        {
            var firstColumn = new JObject();
            firstColumn["name"] = "Culture";
            firstColumn["label"] = "Culture";
            firstColumn["hidden"] = true;

            var secondColumn = new JObject();
            secondColumn["name"] = "CultureDisplay";
            secondColumn["label"] = T("Culture").Text;
            var formatOpt = new JObject();
            formatOpt["hasView"] = true;
            secondColumn["formatter"] = "cellLinkTemplateWithoutDelete";
            secondColumn["formatoptions"] = formatOpt;

            var thirdColumn = new JObject();
            thirdColumn["name"] = "Translatable";
            thirdColumn["label"] = T("Translatable").Text;

            var fourthColumn = new JObject();
            fourthColumn["name"] = "Translated";
            fourthColumn["label"] = T("Translated").Text;

            var fifthColumn = new JObject();
            fifthColumn["name"] = "Missing";
            fifthColumn["label"] = T("Missing").Text;

            var columns = new List<JObject> { firstColumn, secondColumn, thirdColumn, fourthColumn, fifthColumn };

            var json = JsonConvert.SerializeObject(columns);
            var message = new HttpResponseMessage { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };

            return message;
        }
    }
}