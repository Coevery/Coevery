using System.Collections.Generic;
using JetBrains.Annotations;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Navigation.Models;
using Orchard.Environment.Extensions;
using Orchard.Localization.Services;
using Orchard.UI.Navigation;

namespace Orchard.CulturePicker.Services {
    //[UsedImplicitly]
    //[OrchardFeature("Orchard.CulturePicker.MainMenu")]
    //public class LocalizableMainMenuNavigationFilter : INavigationFilter {
    //    private readonly ICultureManager _cultureManager;
    //    private readonly IWorkContextAccessor _workContextAccessor;

    //    public LocalizableMainMenuNavigationFilter(ICultureManager cultureManager, IWorkContextAccessor workContextAccessor) {
    //        _cultureManager = cultureManager;
    //        _workContextAccessor = workContextAccessor;
    //    }

    //    #region INavigationFilter Members

    //    public IEnumerable<MenuItem> Filter(IEnumerable<MenuItem> menuItems) {
    //        string currentCulture = _cultureManager.GetCurrentCulture(_workContextAccessor.GetContext().HttpContext);
    //        foreach (MenuItem menuItem in menuItems) {
    //            ILocalizableAspect localizationPart = menuItem.Content.Is<ContentMenuItemPart>()
    //                                                      ? menuItem.Content.As<ContentMenuItemPart>().Content.As<ILocalizableAspect>()
    //                                                      : menuItem.Content.As<ILocalizableAspect>();
    //            if (localizationPart == null
    //                || localizationPart.Culture == null
    //                || localizationPart.Culture == currentCulture)
    //                yield return menuItem;
    //        }
    //    }

    //    #endregion
    //}
}