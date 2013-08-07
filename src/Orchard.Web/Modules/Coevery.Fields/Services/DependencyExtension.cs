using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Coevery.Fields.Services {
    public class DependencyValuePair : IEqualityComparer<DependencyValuePair> {
        public const string ControlSplit = "::";
        public const string DependentSplit = ",,";
        public const string PairSplit = ";;";

        public int ControlFieldValue { get; set; }
        public int[] DependentFieldValue { get; set; }

        public bool Equals(DependencyValuePair paramA, DependencyValuePair paramB) {
            return paramA.ControlFieldValue == paramB.ControlFieldValue;
        }

        public int GetHashCode(DependencyValuePair toGet) {
            return toGet.ControlFieldValue;
        }

        public new string ToString() {
            return String.Concat(ControlFieldValue.ToString("D") +
                ControlSplit, String.Join(DependentSplit, DependentFieldValue));
        }
    }

    public static class DependencyExtension {
        public static string DependencyPairsToString(this DependencyValuePair[] pairs) {
            var result = new StringBuilder();
            for (var index = 0; index < pairs.Length; index++) {
                if (index != 0) {
                    result.Append(DependencyValuePair.PairSplit);
                }
                result.Append(pairs[index].ToString());
            }
            return result.ToString();
        }

        public static Dictionary<int, int[]> RecoverValuePairs(this string rawValue) {
            if (string.IsNullOrWhiteSpace(rawValue)) {
                return null;
            }
            var result = new Dictionary<int, int[]>();

            foreach (var pair in rawValue.Split(new string[] { DependencyValuePair.PairSplit },
                             StringSplitOptions.RemoveEmptyEntries)) {
                if (string.IsNullOrWhiteSpace(pair)) {
                    return null;
                }
                var temp = pair.Split(new string[] { DependencyValuePair.ControlSplit },
                                      StringSplitOptions.RemoveEmptyEntries);
                int tempControl;
                if (!int.TryParse(temp[0], out tempControl)) {
                    return null;
                }
                if (string.IsNullOrWhiteSpace(temp[1])) {
                    return null;
                }
                result.Add(tempControl,Array.ConvertAll(temp[1].Split(new string[] { DependencyValuePair.DependentSplit },
                                      StringSplitOptions.RemoveEmptyEntries), int.Parse));
            }
            return result;
        }
    }
}