using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Coevery.Fields.Services {
    public class DependencyValuePair {
        public const string ControlSplit = "::";
        public const string BoolSplit = "__";
        public const string DependentSplit = ",,";
        public const string PairSplit = ";;";

        public string ControlFieldValue { get; set; }
        public int[] DependentFieldValue { get; set; }

        private string SpecializeControlFieldValue(string name) {
            return (ControlFieldValue.ToLower() == bool.TrueString.ToLower()
                    || ControlFieldValue.ToLower() == bool.FalseString.ToLower())
                    ? ControlFieldValue.ToLower() + BoolSplit + name : ControlFieldValue;
        }

        public string ToString(string controlName) {
            return String.Concat(SpecializeControlFieldValue(controlName) +
                ControlSplit, String.Join(DependentSplit, DependentFieldValue));
        }
    }

    public static class DependencyExtension {
        public static string DependencyPairsToString(this DependencyValuePair[] pairs, string controlName) {
            var result = new StringBuilder();
            for (var index = 0; index < pairs.Length; index++) {
                if (index != 0) {
                    result.Append(DependencyValuePair.PairSplit);
                }
                result.Append(pairs[index].ToString(controlName));
            }
            return result.ToString();
        }

        public static Dictionary<string, int[]> RecoverValuePairs(this string rawValue) {
            if (string.IsNullOrWhiteSpace(rawValue)) {
                return null;
            }
            var result = new Dictionary<string, int[]>();

            foreach (var pair in rawValue.Split(new string[] { DependencyValuePair.PairSplit },
                             StringSplitOptions.RemoveEmptyEntries)) {
                if (string.IsNullOrWhiteSpace(pair)) {
                    return null;
                }
                var temp = pair.Split(new string[] { DependencyValuePair.ControlSplit },
                                      StringSplitOptions.RemoveEmptyEntries);

                if (string.IsNullOrWhiteSpace(temp[0]) || string.IsNullOrWhiteSpace(temp[1])) {
                    return null;
                }

                result.Add(temp[0],Array.ConvertAll(temp[1].Split(new string[] { DependencyValuePair.DependentSplit },
                                      StringSplitOptions.RemoveEmptyEntries), int.Parse));
            }
            return result;
        }
    }
}