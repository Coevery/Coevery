namespace Coevery.OptionSet.Models {
    /// <summary>
    /// Contrary to what its named states, it's not a part
    /// but a DTO to carry over a TermPart and a Field name
    /// </summary>
    public class OptionItemContentItemPart {
        public string Field { get; set; }
        public OptionItemPart OptionItemPart { get; set; }
    }
}