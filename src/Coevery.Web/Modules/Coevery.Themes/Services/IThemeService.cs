using Coevery.Environment.Extensions.Models;

namespace Coevery.Themes.Services {
    public interface IThemeService : IDependency {
        void DisableThemeFeatures(string themeName);
        void EnableThemeFeatures(string themeName);
        bool IsRecentlyInstalled(ExtensionDescriptor module);
    }
}