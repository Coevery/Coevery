using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using Coevery.Core.Models;
using Orchard.ContentManagement;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Coevery.Core
{
    /// <summary>
    /// Dynamically injects query results as menu items on NavigationQueryMenuItem elements
    /// </summary>
    public class NavigationQueryProvider : INavigationFilter
    {
        private readonly IContentManager _contentManager;
 

        public NavigationQueryProvider(
            IContentManager contentManager
           )
        {
            _contentManager = contentManager;
        }

        public IEnumerable<MenuItem> Filter(IEnumerable<MenuItem> items)
        {

            foreach (var item in items)
            {
                if (item.Content != null && item.Content.ContentItem.ContentType == "ModuleMenuItem")
                {
                    var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
                    var moduleMenuPart = item.Content.As<ModuleMenuItemPart>();
                    const string urlTemp = "~/Coevery#/{0}";
                    string pluralContentTypeName = pluralService.Pluralize(moduleMenuPart.Record.ContentTypeDefinitionRecord.Name);
                    string url = string.Format(urlTemp, pluralContentTypeName);
                    item.Url = url;
                    yield return item;
                }
                else
                {
                    yield return item;
                }
            }
        }
    }
}