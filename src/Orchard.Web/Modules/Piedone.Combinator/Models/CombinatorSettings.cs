using System.Text.RegularExpressions;
using Orchard.Environment.Extensions;

namespace Piedone.Combinator.Models
{
    [OrchardFeature("Piedone.Combinator")]
    public class CombinatorSettings : ICombinatorSettings
    {
        public Regex CombinationExcludeFilter { get; set; }
        public bool CombineCDNResources { get; set; }
        public bool MinifyResources { get; set; }
        public Regex MinificationExcludeFilter { get; set; }
        public bool EmbedCssImages { get; set; }
        public int EmbeddedImagesMaxSizeKB { get; set; }
        public Regex EmbedCssImagesStylesheetExcludeFilter { get; set; }
        public Regex[] ResourceSetFilters { get; set; }
    }
}