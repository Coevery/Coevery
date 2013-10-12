using System.Web.Mvc;
using Coevery.Core.Models;
using System.Collections.Generic;

namespace Coevery.Core.ViewModels {
    public class ModuleMenuItemEditViewModel {
        public int ContenTypeId { get; set; }
        public ModuleMenuItemPart Part { get; set; }
        public IEnumerable<SelectListItem> ContentTypes { get; set; }
    }
}