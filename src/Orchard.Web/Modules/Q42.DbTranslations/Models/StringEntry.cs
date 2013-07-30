using System.Collections.Generic;

namespace Q42.DbTranslations.Models
{
  public class StringEntry
  {
    public string Context { get; set; }
    public string Key { get; set; }
    public string English { get; set; }
    public string Translation { get; set; }
    public bool Used { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public string Culture { get; set; }
    /// <summary>
    /// Original path to .PO file for backwards compatibility
    /// </summary>
    public string Path { get; set; }
  }

  public class StringEntryEqualityComparer : IEqualityComparer<StringEntry>
  {
    public bool Equals(StringEntry x, StringEntry y)
    {
      if (x == null) return y == null;
      if (y == null) return false;
      return x.Key == y.Key && x.Context == y.Context;
    }

    public int GetHashCode(StringEntry obj)
    {
      return obj.Key.GetHashCode() ^ obj.Context.GetHashCode();
    }
  }
}
