using Coevery.ContentManagement.Handlers;
using Coevery.Core.Navigation.Models;
using Coevery.Data;

namespace Coevery.Core.Navigation.Handlers {
    public class MenuWidgetPartHandler : ContentHandler {
        public MenuWidgetPartHandler(IRepository<MenuWidgetPartRecord> repository) {
            Filters.Add(StorageFilter.For(repository));

            OnInitializing<MenuWidgetPart>((context, part) => { part.StartLevel = 1; });
        }
    }
}