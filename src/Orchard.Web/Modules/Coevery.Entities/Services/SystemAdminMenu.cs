using Orchard.ContentManagement.MetaData;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Coevery.Entities.Services
{
    public class SystemAdminMenu : INavigationProvider {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        public Localizer T { get; set; }
        public string MenuName { get { return "SystemAdmin"; } }

        public SystemAdminMenu(IContentDefinitionManager contentDefinitionManager) {
            _contentDefinitionManager = contentDefinitionManager;
        }

        public void GetNavigation(NavigationBuilder builder) {
            builder.AddImageSet("Entities")
                   .Add(T("Entities"), "2",
                        menu => {
                            int menuIdex = 0;
                            menu.Add(T("All Entities"), (++menuIdex).ToString(), item => item.Url("~/SystemAdmin#/Entities"));
                            var userDefinedTypes = _contentDefinitionManager.ListUserDefinedTypeDefinitions();
                            foreach (var type in userDefinedTypes) {
                                var typeModel = type;
                                menu.Add(T(typeModel.DisplayName), (++menuIdex).ToString(), item => item.Url("~/SystemAdmin#/Entities/" + typeModel.Name));
                            }
                        });
        }
    }
}