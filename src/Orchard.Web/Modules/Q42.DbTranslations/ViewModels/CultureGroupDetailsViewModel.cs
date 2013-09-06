using System.Collections.Generic;
using System.Linq;

namespace Q42.DbTranslations.ViewModels
{
  public class CultureGroupDetailsViewModel
  {
    public CultureGroupDetailsViewModel()
    {
      Groups = new List<TranslationGroup>();
      CurrentGroupTranslations = new List<IGrouping<string, TranslationViewModel>>();
    }

    public string Culture { get; set; }
    public IList<TranslationGroup> Groups { get; set; }
    public string CurrentGroupPath { get; set; }
    public IEnumerable<IGrouping<string, TranslationViewModel>> CurrentGroupTranslations { get; set; }
    public bool CanTranslate { get; set; }

    public class TranslationViewModel
    {
      public int Id { get; set; }
      public string GroupPath { get; set; }
      public string Context { get; set; }
      public string Key { get; set; }
      public string OriginalString { get; set; }
      public string LocalString { get; set; }
      public bool ExistsInEnglish { get; set; }
    }

    public class TranslationGroup
    {
      public string Path { get; set; }
      public int TotalCount { get; set; }
      public int TranslationCount { get; set; }
    }
  }
}