using System;

namespace Coevery.Environment.Extensions {
    [AttributeUsage(AttributeTargets.Class)]
    public class CoeveryFeatureAttribute : Attribute {
        public CoeveryFeatureAttribute(string text) {
            FeatureName = text;
        }

        public string FeatureName { get; set; }
    }
}