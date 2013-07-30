using System.Collections.Generic;

namespace Q42.DbTranslations.ViewModels
{
  public class CultureDetailsViewModel
  {
    public CultureDetailsViewModel()
    {
      Groups = new List<TranslationGroupViewModel>();
    }

    public string Culture { get; set; }
    public IList<TranslationGroupViewModel> Groups { get; set; }

    public class TranslationViewModel
    {
      public string Context { get; set; }
      public string Key { get; set; }
      public string OriginalString { get; set; }
      public string LocalString { get; set; }
    }

    public class TranslationGroupViewModel
    {
      public TranslationGroupViewModel()
      {
        Translations = new List<TranslationViewModel>();
      }

      public string Path { get; set; }
      public IList<TranslationViewModel> Translations { get; set; }
    }
  }
}