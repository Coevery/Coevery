using System.Collections.Generic;

namespace Coevery.Translations.ViewModels
{
  public class CultureIndexViewModel
  {
    public CultureIndexViewModel()
    {
      TranslationStates = new Dictionary<string, CultureTranslationState>();
    }

    public Dictionary<string, CultureTranslationState> TranslationStates { get; protected set; }
    public int NumberOfStringsInDefaultCulture { get; set; }

    public class CultureTranslationState
    {
      public int NumberOfTranslatedStrings { get; set; }
    }
  }
}