using System.Collections.Generic;
using Coevery.Environment.Extensions.Models;
using Coevery.Localization;

namespace Coevery.Modules.Models {
    public class FeatureEntry {
        public string FeatureId { get; set; }
        public string FeatureName { get; set; }
        public string FeatureDescription { get; set; }
        public string FeatureState { get; set; }
        public bool IsRecentlyInstalled { get; set; }
        public bool NeedsUpdate { get; set; }
        public IEnumerable<ModuleFeature> Dependencies { get; set; }
        public string TitleStyle { get; set; }
        public FeatureAction FeatureAction { get; set; }
    }

    public class FeatureCategory {
        public LocalizedString CategoryName { get; set; }
        public IEnumerable<FeatureEntry> Features { get; set; }
    }

    public class FeatureAction {
        public bool IsShow { get; set; }
        public string Action { get; set; }
        public bool Force { get; set; }
        public string Iconclass { get; set; }
        public LocalizedString Content { get; set; }
    }

    public enum FeaturesBulkAction {
        None,
        Enable,
        Disable,
        Update,
        Toggle
    }
}