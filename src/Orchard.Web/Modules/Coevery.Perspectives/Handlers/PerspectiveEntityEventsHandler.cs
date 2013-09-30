using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Web;
using Coevery.Entities.Events;
using Orchard.ContentManagement;
using Orchard.Core.Navigation.Models;

namespace Coevery.Perspectives.Handlers {
    public class PerspectiveEntityEventsHandler : IEntityEvents {
        private readonly IContentManager _contentManager;

        public PerspectiveEntityEventsHandler(IContentManager contentManager) {
            _contentManager = contentManager;
        }

        public void OnCreated(string entityName) {}
        public void OnUpdating(string entityName) {
        }

        public void OnDeleting(string entityName) {
            var pluralService = PluralizationService.CreateService(new CultureInfo("en-US"));
            foreach (var navigation in _contentManager.List<MenuPart>("ModuleMenuItem")) {
                if (navigation.MenuText == pluralService.Pluralize(entityName)) {
                    _contentManager.Remove(navigation.ContentItem);
                }
            }
        }
    }
}