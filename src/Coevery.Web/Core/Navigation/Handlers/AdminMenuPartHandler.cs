using System;
using JetBrains.Annotations;
using Coevery.ContentManagement.Handlers;
using Coevery.Core.Navigation.Models;
using Coevery.Data;

namespace Coevery.Core.Navigation.Handlers {
    [UsedImplicitly]
    public class AdminMenuPartHandler : ContentHandler {
        public AdminMenuPartHandler(IRepository<AdminMenuPartRecord> menuPartRepository) {
            Filters.Add(StorageFilter.For(menuPartRepository));

            OnInitializing<AdminMenuPart>((ctx, x) => {
                                      x.OnAdminMenu = false;
                                      x.AdminMenuText = String.Empty;
                                  });
        }
    }
}