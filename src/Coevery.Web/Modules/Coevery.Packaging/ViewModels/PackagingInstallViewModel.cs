using System.Collections.Generic;
using Coevery.Environment.Extensions.Models;
using Coevery.Recipes.Models;

namespace Coevery.Packaging.ViewModels {
    public class PackagingInstallViewModel {
        public List<PackagingInstallFeatureViewModel> Features { get; set; }
        public List<PackagingInstallRecipeViewModel> Recipes { get; set; }

        public ExtensionDescriptor ExtensionDescriptor { get; set; }
    }

    public class PackagingInstallFeatureViewModel {
        public FeatureDescriptor FeatureDescriptor { get; set; }
        public bool Enable { get; set; }
    }

    public class PackagingInstallRecipeViewModel {
        public Recipe Recipe { get; set; }
        public bool Execute { get; set; }
    }
}