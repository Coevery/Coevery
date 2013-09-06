using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Orchard.Caching;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Orchard.FileSystems.WebSite;
using Orchard.Localization.Services;

namespace Q42.DbTranslations.Services
{
  [OrchardSuppressDependency("Orchard.Localization.Services.DefaultLocalizedStringManager")]
  public class LiveLocalizedStringManager : ILocalizedStringManager
  {
    private readonly IWebSiteFolder _webSiteFolder;
    private readonly IExtensionManager _extensionManager;
    private readonly ICacheManager _cacheManager;
    private readonly ShellSettings _shellSettings;
    private readonly ISignals _signals;
    private readonly ILocalizationService _localizationService;
    const string CoreLocalizationFilePathFormat = "~/Core/App_Data/Localization/{0}/orchard.core.po";
    const string ModulesLocalizationFilePathFormat = "~/Modules/{0}/App_Data/Localization/{1}/orchard.module.po";
    const string ThemesLocalizationFilePathFormat = "~/Themes/{0}/App_Data/Localization/{1}/orchard.theme.po";
    const string RootLocalizationFilePathFormat = "~/App_Data/Localization/{0}/orchard.root.po";
    const string TenantLocalizationFilePathFormat = "~/App_Data/Sites/{0}/Localization/{1}/orchard.po";

    public LiveLocalizedStringManager(
        IWebSiteFolder webSiteFolder,
        IExtensionManager extensionManager,
        ICacheManager cacheManager,
        ShellSettings shellSettings,
        ISignals signals, ILocalizationService localizationService)
    {
      _localizationService = localizationService;
      _webSiteFolder = webSiteFolder;
      _extensionManager = extensionManager;
      _cacheManager = cacheManager;
      _shellSettings = shellSettings;
      _signals = signals;
    }

    // This will translate a string into a string in the target cultureName.
    // The scope portion is optional, it amounts to the location of the file containing 
    // the string in case it lives in a view, or the namespace name if the string lives in a binary.
    // If the culture doesn't have a translation for the string, it will fallback to the 
    // parent culture as defined in the .net culture hierarchy. e.g. fr-FR will fallback to fr.
    // In case it's not found anywhere, the text is returned as is.
    public string GetLocalizedString(string scope, string text, string cultureName)
    {
      var culture = LoadCulture(cultureName);
      string scopedKey = !string.IsNullOrWhiteSpace(scope) ? (scope + "|" + text).ToLowerInvariant() : text.ToLowerInvariant();
      if (culture.Translations.ContainsKey(scopedKey))
        return culture.Translations[scopedKey];

      string genericKey = ("|" + text).ToLowerInvariant();
      if (culture.Translations.ContainsKey(genericKey))
        return culture.Translations[genericKey];

      string retVal = GetParentTranslation(scope, text, cultureName);

      if (retVal == text && culture.Translations.Any(s => s.Key.Contains("|" + scopedKey)))
        retVal = culture.Translations.First(s => s.Key.Contains("|" + scopedKey)).Value;

      return retVal;
    }

    private string GetParentTranslation(string scope, string text, string cultureName)
    {
      string scopedKey = (scope + "|" + text).ToLowerInvariant();
      string genericKey = ("|" + text).ToLowerInvariant();
      try
      {
        CultureInfo cultureInfo = CultureInfo.GetCultureInfo(cultureName);
        CultureInfo parentCultureInfo = cultureInfo.Parent;
        if (parentCultureInfo.IsNeutralCulture)
        {
          var culture = LoadCulture(parentCultureInfo.Name);
          if (culture.Translations.ContainsKey(scopedKey))
          {
            return culture.Translations[scopedKey];
          }
          if (culture.Translations.ContainsKey(genericKey))
          {
            return culture.Translations[genericKey];
          }
          return text;
        }
      }
      catch (CultureNotFoundException) { }

      return text;
    }

    // Loads the culture dictionary in memory and caches it.
    // Cache entry will be invalidated any time the directories hosting 
    // the .po files are modified.
    private CultureDictionary LoadCulture(string culture)
    {
      return _cacheManager.Get(culture, ctx =>
      {
        ctx.Monitor(_signals.When("culturesChanged" + _shellSettings.Name));
        return new CultureDictionary
        {
          CultureName = culture,
          Translations = LoadTranslationsForCulture(culture, ctx)
        };
      });
    }

    private IDictionary<string, string> LoadTranslationsForCulture(string culture, AcquireContext<string> context)
    {
      IDictionary<string, string> result = new Dictionary<string, string>();

      var translations = _localizationService.GetTranslations(culture);
      foreach (var t in translations)
      {
        var scope = t.Context;
        var id = t.Key;
        string scopedKey = (scope + "|" + id).ToLowerInvariant();
        if (!result.ContainsKey(scopedKey))
        {
          result.Add(scopedKey, t.Translation);
        }
        else
        {
          result[scopedKey] = t.Translation;
        }
      }

      return result;
    }

    private static readonly Dictionary<char, char> _escapeTranslations = new Dictionary<char, char> {
            { 'n', '\n' },
            { 'r', '\r' },
            { 't', '\t' }
        };

    private static string Unescape(string str)
    {
      StringBuilder sb = null;
      bool escaped = false;
      for (var i = 0; i < str.Length; i++)
      {
        var c = str[i];
        if (escaped)
        {
          if (sb == null)
          {
            sb = new StringBuilder(str.Length);
            if (i > 1)
            {
              sb.Append(str.Substring(0, i - 1));
            }
          }
          char unescaped;
          if (_escapeTranslations.TryGetValue(c, out unescaped))
          {
            sb.Append(unescaped);
          }
          else
          {
            // General rule: \x ==> x
            sb.Append(c);
          }
          escaped = false;
        }
        else
        {
          if (c == '\\')
          {
            escaped = true;
          }
          else if (sb != null)
          {
            sb.Append(c);
          }
        }
      }
      return sb == null ? str : sb.ToString();
    }

    class CultureDictionary
    {
      public string CultureName { get; set; }
      public IDictionary<string, string> Translations { get; set; }
    }
  }
}
