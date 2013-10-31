using Coevery.Environment;
using Coevery.Environment.Extensions;
using Coevery.Environment.Extensions.Models;
using Coevery.Localization;
using Coevery.Packaging.Services;

namespace Coevery.Packaging {
    [CoeveryFeature("Gallery")]
    public class DefaultPackagingUpdater : IFeatureEventHandler {
        private readonly IPackagingSourceManager _packagingSourceManager;

        public DefaultPackagingUpdater(IPackagingSourceManager packagingSourceManager) {
            _packagingSourceManager = packagingSourceManager;
        }

        public Localizer T { get; set; }

        public void Installing(Feature feature) {
        }

        public void Installed(Feature feature) {
            if (feature.Descriptor.Id == "Gallery") {
                _packagingSourceManager.AddSource("Coevery Gallery", "http://packages.coeveryproject.net/FeedService.svc");
            }
        }

        public void Enabling(Feature feature) {
        }

        public void Enabled(Feature feature) {
        }

        public void Disabling(Feature feature) {
        }

        public void Disabled(Feature feature) {
        }

        public void Uninstalling(Feature feature) {
        }

        public void Uninstalled(Feature feature) {
        }
    }
}