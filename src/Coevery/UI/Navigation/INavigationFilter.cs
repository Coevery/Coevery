using System.Collections.Generic;

namespace Coevery.UI.Navigation {
    /// <summary>
    /// Provides a way to alter the main navigation, for instance by dynamically injecting new items
    /// </summary>
    public interface INavigationFilter : IDependency {
        IEnumerable<MenuItem> Filter(IEnumerable<MenuItem> menuItems);
    }
}
