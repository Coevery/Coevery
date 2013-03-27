using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Web;
using Coevery.Core.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Navigation.Models;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Coevery.Core.Services
{
    public class ModuelMenuProvider : IMenuProvider
    {
        private readonly IContentManager _contentManager;

        public ModuelMenuProvider(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public void GetMenu(IContent menu, NavigationBuilder builder)
        {
            var menuParts = _contentManager
                .Query<MenuPart, MenuPartRecord>()
                .Where(x => x.MenuId == menu.Id)
                .WithQueryHints(new QueryHints().ExpandRecords<MenuItemPartRecord>())
                .List();

            foreach (var menuPart in menuParts)
            {
                if (menuPart != null)
                {
                    var part = menuPart;

                    string culture = null;
                    // fetch the culture of the content menu item, if any
                    var localized = part.As<ILocalizableAspect>();
                    if (localized != null)
                    {
                        culture = localized.Culture;
                    }
                    else
                    {
                        // fetch the culture of the content menu item, if any
                        var contentMenuItemPart = part.As<ContentMenuItemPart>();
                        if (contentMenuItemPart != null)
                        {
                            localized = contentMenuItemPart.Content.As<ILocalizableAspect>();
                            if (localized != null)
                            {
                                culture = localized.Culture;
                            }
                        }
                    }

                    if (part.Is<ModuleMenuItemPart>())
                    {
                        var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
                        var moduleMenuPart = part.As<ModuleMenuItemPart>();
                        const string urlTemp = "~/Coevery#/{0}";
                        string pluralContentTypeName = pluralService.Pluralize(moduleMenuPart.Record.ContentTypeDefinitionRecord.Name);
                        string url = string.Format(urlTemp, pluralContentTypeName);
                        builder.Add(new LocalizedString(HttpUtility.HtmlEncode(part.MenuText)), part.MenuPosition, item => item.Url(url).Content(part).Culture(culture).Permission(Orchard.Core.Contents.Permissions.ViewContent));
                    }

                }
            }
        }
    }
}