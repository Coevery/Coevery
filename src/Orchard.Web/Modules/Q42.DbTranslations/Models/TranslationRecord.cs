namespace Q42.DbTranslations.Models
{
  public class TranslationRecord
  {
    public virtual int Id { get; set; }
    public virtual string Culture { get; set; }
    public virtual string Value { get; set; }
    public virtual LocalizableStringRecord LocalizableStringRecord { get; set; }
  }
}
