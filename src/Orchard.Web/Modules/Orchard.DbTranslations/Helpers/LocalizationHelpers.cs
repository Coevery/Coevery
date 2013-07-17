using System;
using System.Web;
using Q42.DbTranslations.ViewModels;

namespace Q42.DbTranslations.Helpers
{
  public class LocalizationHelpers
  {
    public static string GetPoFileName(string path)
    {
      if (path.StartsWith("Modules/", StringComparison.OrdinalIgnoreCase))
      {
        return path.Substring(8, path.IndexOf('/', 8) - 8);
      }
      if (path.StartsWith("Themes/", StringComparison.OrdinalIgnoreCase))
      {
        return path.Substring(7, path.IndexOf('/', 7) - 7);
      }
      if (path.EndsWith("orchard.root.po", StringComparison.OrdinalIgnoreCase))
      {
        return "Root";
      }
      if (path.EndsWith("orchard.core.po", StringComparison.OrdinalIgnoreCase))
      {
        return "Core";
      }
      return path;
    }

    public static HtmlString BuildStyleAttribute(CultureGroupDetailsViewModel.TranslationViewModel model)
    {
      if (!model.ExistsInEnglish) return new HtmlString(" style=\"color:orange\"");
      if (String.IsNullOrWhiteSpace(model.LocalString)) return new HtmlString(" style=\"color:red\"");
      return new HtmlString("");
    }
  }
}
