using System.Collections.Generic;

namespace Q42.DbTranslations.Models
{
  public class LocalizableStringRecord
  {
    public LocalizableStringRecord()
    {
      Translations = new List<TranslationRecord>();
    }

    public virtual int Id { get; set; }
    public virtual string Path { get; set; }
    public virtual string Context { get; set; }
    public virtual string StringKey { get; set; }
    public virtual string OriginalLanguageString { get; set; }
    public virtual IList<TranslationRecord> Translations { get; protected set; }

    public virtual void AddTranslation(TranslationRecord translationRecord)
    {
      translationRecord.LocalizableStringRecord = this;
      Translations.Add(translationRecord);
    }
  }
}