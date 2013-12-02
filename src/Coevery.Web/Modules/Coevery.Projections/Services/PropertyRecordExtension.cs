using System.Text.RegularExpressions;
using Coevery.Projections.Models;

namespace Coevery.Projections.Services
{
    public static class PropertyRecordExtension {
        public static string GetFieldName(this PropertyRecord property) {
            string type = property.Type;
            var pattern = @"[^\.]*\.(.*)\.[^\.]*";
            var regex = new Regex(pattern, RegexOptions.Compiled);

            foreach (Match myMatch in regex.Matches(type)) {
                if (myMatch.Success) {
                    return myMatch.Groups[1].Value;
                }
            }
            return type;
        }
        public static string GetFieldName(this string propertyName) {
            var pattern = @"[^\.]*\.(.*)\.[^\.]*";
            var regex = new Regex(pattern, RegexOptions.Compiled);

            foreach (Match myMatch in regex.Matches(propertyName)) {
                if (myMatch.Success) {
                    return myMatch.Groups[1].Value;
                }
            }
            return propertyName;
        }
    }
}