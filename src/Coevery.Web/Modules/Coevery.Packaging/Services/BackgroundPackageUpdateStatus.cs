using Coevery.Environment.Extensions;
using Coevery.Packaging.Models;

namespace Coevery.Packaging.Services {
    public interface IBackgroundPackageUpdateStatus : ISingletonDependency {
        PackagesStatusResult Value { get; set; }
    }

    [CoeveryFeature("Gallery.Updates")]
    public class BackgroundPackageUpdateStatus : IBackgroundPackageUpdateStatus {
        public PackagesStatusResult Value { get; set; }
    }
}