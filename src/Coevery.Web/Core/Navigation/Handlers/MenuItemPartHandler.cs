using JetBrains.Annotations;
using Coevery.ContentManagement;
using Coevery.Core.Navigation.Models;
using Coevery.Data;
using Coevery.ContentManagement.Handlers;

namespace Coevery.Core.Navigation.Handlers {
    [UsedImplicitly]
    public class MenuItemPartHandler : ContentHandler {
        public MenuItemPartHandler(IRepository<MenuItemPartRecord> repository) {
            Filters.Add(new ActivatingFilter<MenuItemPart>("MenuItem"));
            Filters.Add(StorageFilter.For(repository));
        }
    }
}