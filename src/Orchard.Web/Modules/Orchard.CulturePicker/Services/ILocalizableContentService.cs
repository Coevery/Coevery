using Orchard.Autoroute.Models;
using Orchard.ContentManagement;

namespace Orchard.CulturePicker.Services {
    public interface ILocalizableContentService : IDependency {
        bool TryFindLocalizedRoute(ContentItem routableContent, string cultureName, out AutoroutePart localizedRoute);
        bool TryGetRouteForUrl(string url, out AutoroutePart route);
    }
}